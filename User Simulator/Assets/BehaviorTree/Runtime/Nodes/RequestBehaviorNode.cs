using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class RequestBehaviorNode : SubtreeNode
{
    public override BehaviorTree Subtree
    {
        get
        {
            return subtree;
        }

        set
        {
            Debug.LogError("Can't direct assign RequestBehaviorNode subtree. Use SubtreeNode instead.");
        }
    }

    public UnityEngine.Object tagProvider;
    public List<BTagParameter> minimumValueParameters = new();
    public List<BTagParameter> maximumValueParameters = new();

    BehaviorTag currentTag;

    BehaviorTag requestTag()
    {
        IBTagProvider provider = tagProvider as IBTagProvider;
        List<BehaviorTag> tags = provider.ProvideTags(tree.bTagParameters);

        foreach (BehaviorTag tag in tags)
        {
            if(BTagParameter.IsCompatible(tag.parameters, minimumValueParameters, maximumValueParameters))
            {
                return tag;
            }
        }

        return null;
    }

    public override void OnStart()
    {
        if(currentTag == null)
        {
            currentTag = requestTag();
            if(currentTag != null)
            {
                subtree = currentTag.tree;
                ValidateSubtree();
            }
            
        }

        base.OnStart();
    }

    public override NodeState OnUpdate()
    {
        if(currentTag == null)
        {
            return NodeState.Failure;
        }

        return base.OnUpdate();
    }

    void OnValidate()
    {
        if(tagProvider is not IBTagProvider)
        {
            tagProvider = null;
        }
    }

}
