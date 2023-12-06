using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class ComponentActivationMixerBehaviour : PlayableBehaviour
{
    bool m_DefaultEnabled;

    bool m_AssignedEnabled;

    Behaviour m_TrackBinding;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        m_TrackBinding = playerData as Behaviour;

        if (m_TrackBinding == null)
            return;

        int inputCount = playable.GetInputCount ();

        bool isEnabled = false;
        for (int i = 0; i < inputCount; i++)
        {
            float inputWeight = playable.GetInputWeight(i);

            if (!Mathf.Approximately (inputWeight, 0f))
            {
                isEnabled = true;
                break;
            }
                
        }

        if (isEnabled)
        {
            m_TrackBinding.enabled = true;
        }
        else
        {
            m_TrackBinding.enabled = false;
        }
    }
}