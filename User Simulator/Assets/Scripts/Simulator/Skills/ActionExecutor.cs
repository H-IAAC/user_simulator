using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Playables;
using HIAAC.CstUnity.Core.Entities;

/// <summary>
/// Executes motor skills requested by an external agent over Redis.
/// Animation skills replay the existing action Timelines (skill -> PlayableDirector);
/// walk_to drives the NavMeshAgent. Receives command lists from the action_command
/// memory, runs them sequentially and publishes progress on action_status.
/// </summary>
public class ActionExecutor : MonoBehaviour
{
    [Header("Skills")]
    [SerializeField] private List<SkillDefinition> skills = new List<SkillDefinition>();

    [Header("CST")]
    [SerializeField] MindReference mindReference;

    [Header("Navigation")]
    [Tooltip("Walk command is failed if the destination is not reached within this time (seconds).")]
    [SerializeField] private float maxWalkTime = 60.0f;

    private NavMeshAgent navMeshAgent;
    private Animator animator;

    private enum ExecutorState
    {
        Idle,
        Walking,
        Animating
    }

    private class CommandEntry
    {
        public string Skill;
        public Vector3 Destination;
        public float Velocity = 3.5f;
    }

    private class CommandPayload
    {
        [JsonProperty("id")] public long Id = 0;
        [JsonProperty("commands")] public List<CommandEntryJson> Commands = new List<CommandEntryJson>();
    }

    private class CommandEntryJson
    {
        [JsonProperty("skill")] public string Skill = "";
        [JsonProperty("parameters")] public JObject Parameters;
    }

    private readonly Queue<CommandEntry> queue = new Queue<CommandEntry>();
    private CommandEntry currentCommand;
    private ExecutorState state = ExecutorState.Idle;
    private PlayableDirector currentDirector;
    private long currentCommandId = 0;
    private int currentCommandIndex = 0;
    private long lastExecutedCommandId = 0;
    private float walkStartTime;

    private const string WalkSkillName = "walk_to";

    // Agent.controller states driven by the executor
    private static readonly int IdleStateHash = Animator.StringToHash("Idle");
    private static readonly int WalkStateHash = Animator.StringToHash("Walking");

    void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    Memory actionCommand;
    Memory actionStatus;

    void Start()
    {
        actionCommand = mindReference.getMemory("ActionCommand", "");
        actionStatus = mindReference.getMemory("ActionStatus", "");
    }

    void Update()
    {
        PollCommands();
        UpdateCurrentCommand();
    }

    private void PollCommands()
    {
        if (mindReference == null) return;

        object payload = actionCommand.getI();
        if (payload == null) return;

        string json = payload.ToString();
        if (string.IsNullOrEmpty(json)) return;

        CommandPayload command;
        try
        {
            command = JsonConvert.DeserializeObject<CommandPayload>(json);
        }
        catch (JsonException e)
        {
            Debug.LogWarning($"[ActionExecutor] Malformed action command: {e.Message}");
            actionCommand.setI("");
            return;
        }

        if (command == null || command.Commands.Count == 0 || command.Id == lastExecutedCommandId)
        {
            actionCommand.setI("");
            return;
        }

        actionCommand.setI("");
        lastExecutedCommandId = command.Id;

        EnqueueCommands(command);
    }

    private void EnqueueCommands(CommandPayload payload)
    {
        // Preempt: abort whatever is running and replace the queue
        AbortCurrentCommand();
        queue.Clear();

        currentCommandId = payload.Id;
        currentCommandIndex = -1;

        foreach (CommandEntryJson entry in payload.Commands)
        {
            CommandEntry command = ParseCommandEntry(entry);
            if (command != null) queue.Enqueue(command);
        }

        StartNextCommand();
    }

    private CommandEntry ParseCommandEntry(CommandEntryJson entry)
    {
        CommandEntry command = new CommandEntry
        {
            Skill = entry.Skill
        };

        if (entry.Skill == WalkSkillName)
        {
            if (entry.Parameters?["destination"] is JArray destination && destination.Count >= 3)
            {
                command.Destination = new Vector3(
                    destination[0].Value<float>(),
                    destination[1].Value<float>(),
                    destination[2].Value<float>());
            }
            else
            {
                Debug.LogWarning("[ActionExecutor] walk_to without a valid destination parameter");
                return null;
            }

            if (entry.Parameters?["velocity"] != null)
            {
                command.Velocity = entry.Parameters["velocity"].Value<float>();
            }
        }

        return command;
    }

    private void StartNextCommand()
    {
        currentCommandIndex++;

        if (queue.Count == 0)
        {
            currentCommand = null;
            state = ExecutorState.Idle;
            EnterIdle();
            return;
        }

        currentCommand = queue.Dequeue();

        if (currentCommand.Skill == WalkSkillName)
        {
            StartWalk(currentCommand);
        }
        else
        {
            StartAnimation(currentCommand);
        }
    }

    private void StartWalk(CommandEntry command)
    {
        SkillDefinition skill = skills.Find(s => s.skillName == command.Skill);

        // walk_to is always available, even when not listed
        if (skill != null && skill.director != null)
        {
            Debug.LogWarning($"[ActionExecutor] Skill '{command.Skill}' conflicts with walk_to, skipping");
            PublishStatus("failed");
            StartNextCommand();
            return;
        }

        navMeshAgent.enabled = true;

        if (!navMeshAgent.isOnNavMesh)
        {
            Debug.LogWarning($"[ActionExecutor] walk_to: agent is not on the NavMesh (destination {command.Destination})");
            PublishStatus("failed");
            StartNextCommand();
            return;
        }

        navMeshAgent.speed = command.Velocity;

        if (!navMeshAgent.SetDestination(command.Destination))
        {
            Debug.LogWarning($"[ActionExecutor] walk_to: destination unreachable ({command.Destination})");
            PublishStatus("failed");
            StartNextCommand();
            return;
        }

        walkStartTime = Time.time;

        if (animator != null)
        {
            animator.Play(WalkStateHash, 0, 0f);
        }

        state = ExecutorState.Walking;
        PublishStatus("running");
    }

    private void EnterIdle()
    {
        if (navMeshAgent != null)
        {
            navMeshAgent.enabled = true;
            navMeshAgent.ResetPath();
        }

        if (animator != null)
        {
            animator.Play(IdleStateHash, 0, 0f);
        }
    }

    private void StartAnimation(CommandEntry command)
    {
        SkillDefinition skill = skills.Find(s => s.skillName == command.Skill);

        if (skill == null || skill.director == null)
        {
            Debug.LogWarning($"[ActionExecutor] Unknown skill '{command.Skill}', skipping");
            PublishStatus("failed");
            StartNextCommand();
            return;
        }

        navMeshAgent.enabled = false;

        currentDirector = skill.director;
        currentDirector.Stop();
        currentDirector.Play();

        state = ExecutorState.Animating;
        PublishStatus("running");
    }

    private void UpdateCurrentCommand()
    {
        switch (state)
        {
            case ExecutorState.Idle:
                if (animator != null &&
                    animator.GetCurrentAnimatorStateInfo(0).shortNameHash != IdleStateHash)
                {
                    animator.Play(IdleStateHash, 0, 0f);
                }
                break;
            case ExecutorState.Walking:
                UpdateWalk();
                break;
            case ExecutorState.Animating:
                UpdateAnimation();
                break;
        }
    }

    private void UpdateWalk()
    {
        if (navMeshAgent.pathPending) return;

        // Only check pathStatus when hasPath is true (Unity docs: pathStatus only valid then)
        if (navMeshAgent.hasPath && navMeshAgent.pathStatus == NavMeshPathStatus.PathInvalid)
        {
            Debug.LogWarning($"[ActionExecutor] walk_to path invalid (destination {currentCommand.Destination})");
            PublishStatus("failed");
            StartNextCommand();
            return;
        }

        if (Time.time - walkStartTime > maxWalkTime)
        {
            Debug.LogWarning($"[ActionExecutor] walk_to timed out after {maxWalkTime}s");
            PublishStatus("failed");
            StartNextCommand();
            return;
        }

        // Arrived: within stopping distance AND (path consumed OR velocity is near zero)
        bool arrived = navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance + 0.1f
                       && (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude < 0.01f);

        if (arrived)
        {
            navMeshAgent.ResetPath();
            PublishStatus("completed");
            StartNextCommand();
        }
    }

    private void UpdateAnimation()
    {
        // Same completion signal as PlayDirectorNode: Hold wrap mode pauses at the end
        if (currentDirector.state != PlayState.Paused) return;

        currentDirector.Stop(); // releases the Animator so Agent.controller resumes walking
        currentDirector = null;
        navMeshAgent.enabled = true;

        PublishStatus("completed");
        StartNextCommand();
    }

    private void AbortCurrentCommand()
    {
        switch (state)
        {
            case ExecutorState.Walking:
                navMeshAgent.ResetPath();
                PublishStatus("interrupted");
                break;
            case ExecutorState.Animating:
                if (currentDirector != null) currentDirector.Stop();
                currentDirector = null;
                navMeshAgent.enabled = true;
                PublishStatus("interrupted");
                break;
        }
    }

    private void PublishStatus(string status)
    {
        if (mindReference == null) return;

        string json = JsonConvert.SerializeObject(new
        {
            id = currentCommandId,
            index = currentCommandIndex,
            skill = currentCommand?.Skill,
            state = status
        });

        actionStatus.setI(json);
    }

    /// <summary>
    /// Manifest published by SkillsExporter: the configured skills plus the built-in walk_to.
    /// </summary>
    public string BuildManifestJson()
    {
        List<object> manifestSkills = new List<object>();

        foreach (SkillDefinition skill in skills)
        {
            manifestSkills.Add(new
            {
                name = skill.skillName,
                type = "animation",
                duration = skill.director != null ? skill.director.duration : 0.0,
                parameters = skill.parameters,
                animations = skill.AnimationsDictionary
            });
        }

        manifestSkills.Add(new
        {
            name = WalkSkillName,
            type = "navigation",
            parameters = new object[]
            {
                new { name = "destination", type = "vec3" },
                new { name = "velocity", type = "float" }
            }
        });

        Debug.Log($"Skills list: {JsonConvert.SerializeObject(manifestSkills)}");

        return JsonConvert.SerializeObject(new
        {
            agent = gameObject.name,
            skills = manifestSkills
        });
    }

    /// <summary>
    /// Debug helper: inject a command list directly, bypassing Redis.
    /// </summary>
    public void ExecuteForDebug(string[] skillNames, Vector3 destination = default, float velocity = 3.5f)
    {
        CommandPayload payload = new CommandPayload
        {
            Id = lastExecutedCommandId + 1,
            Commands = new List<CommandEntryJson>()
        };

        foreach (string skillName in skillNames)
        {
            CommandEntryJson entry = new CommandEntryJson { Skill = skillName };

            if (skillName == WalkSkillName)
            {
                entry.Parameters = new JObject
                {
                    ["destination"] = new JArray { destination.x, destination.y, destination.z },
                    ["velocity"] = velocity
                };
            }

            payload.Commands.Add(entry);
        }

        EnqueueCommands(payload);
    }
}
