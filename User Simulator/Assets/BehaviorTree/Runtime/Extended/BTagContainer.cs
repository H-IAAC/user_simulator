using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName="Behavior Tree/Behavior Tag Container")]
public class BTagContainer :  ScriptableObject, IBTagProvider
{
    public List<BehaviorTag> tags;

    public List<BehaviorTag> ProvideTags(List<BTagParameter> agentParameters)
    {
        List<BehaviorTag> availableTags = new();
        foreach(BehaviorTag tag in tags)
        {
            if(BTagParameter.IsCompatible(agentParameters, tag.minimumValueParameters, tag.maximumValueParameters))
            {
                availableTags.Add(tag);
            }
        }

        return availableTags;
    }
}