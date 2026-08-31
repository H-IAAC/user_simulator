using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Playables;

[Serializable]
public class SkillParameter
{
    public string name;
    public string type;
}

/// <summary>
/// Named string pair. Replaces a Dictionary because Unity cannot serialize dictionaries.
/// </summary>
[Serializable]
public class NamedString
{
    public string key;
    public string value;

    public NamedString() { }

    public NamedString(string key, string value)
    {
        this.key = key;
        this.value = value;
    }
}

/// <summary>
/// A motor skill the agent can perform. Animation skills map to a PlayableDirector
/// playing a Timeline (e.g. the PlayWork/PlayCoffee/PlayBathroom action Timelines);
/// navigation skills (walk_to) are added programmatically by ActionExecutor.
/// </summary>
[Serializable]
public class SkillDefinition
{
    public string skillName;

    [Tooltip("Director playing this skill's Timeline. Leave null for skills handled by the executor itself (e.g. walk_to).")]
    public PlayableDirector director;

    [Tooltip("Character/prop name -> clip name, shown in the skill manifest (e.g. agent -> 'Sitting Idle').")]
    public List<NamedString> animations = new List<NamedString>();

    [Tooltip("Skill signature: parameters the external agent must provide when requesting this skill.")]
    public List<SkillParameter> parameters = new List<SkillParameter>();

    public Dictionary<string, string> AnimationsDictionary =>
        animations.ToDictionary(pair => pair.key, pair => pair.value);
}
