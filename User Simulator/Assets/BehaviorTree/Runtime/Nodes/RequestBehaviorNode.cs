using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class RequestBehaviorNode : SubtreeNode
{
    public RequestBehaviorNode() : base()
    {
        propertiesDontDeleteOnValidate.Add("tagProvider");
    }

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

    public List<BTagParameter> minimumValueParameters = new();
    public List<BTagParameter> maximumValueParameters = new();

    public override void OnCreateProperties()
    {
        CreateProperty(typeof(TagProviderProperty), "tagProvider");
        passValue.Add(false);
    }

    BehaviorTag currentTag;

    BehaviorTag requestTag()
    {
        object providerObj = GetPropertyValue("tagProvider");
        
        IBTagProvider provider = providerObj as IBTagProvider;

        if(provider == null)
        {
            return null;
        }

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

}
