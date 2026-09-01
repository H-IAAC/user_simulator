using UnityEngine;
using HIAAC.CstUnity.Core.Entities;

/// <summary>
/// Publishes the agent's skill manifest to Redis (skill_manifest memory) so the
/// external agent knows which actions it can request.
/// </summary>
public class SkillsExporter : MonoBehaviour
{
    [SerializeField] private ActionExecutor executor;

    [SerializeField] MindReference mindReference;

    Memory skillManifest;

    void Start()
    {
        if (executor == null)
        {
            executor = GetComponent<ActionExecutor>();
        }

        if (executor == null || mindReference == null)
        {
            Debug.LogWarning("[SkillsExporter] Missing ActionExecutor or MindReference reference");
            return;
        }

        skillManifest = mindReference.getMemory("SkillManifest", "");
        skillManifest.setI(executor.BuildManifestJson());
    }
}
