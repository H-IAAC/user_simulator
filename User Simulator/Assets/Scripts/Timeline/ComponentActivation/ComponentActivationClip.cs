using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class ComponentActivationClip : PlayableAsset, ITimelineClipAsset
{
    public ComponentActivationBehaviour template = new ComponentActivationBehaviour ();

    public ClipCaps clipCaps
    {
        get { return ClipCaps.None; }
    }

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<ComponentActivationBehaviour>.Create (graph, template);
        return playable;
    }
}