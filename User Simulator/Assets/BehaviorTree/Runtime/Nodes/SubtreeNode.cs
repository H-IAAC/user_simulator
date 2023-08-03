using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SubtreeNode : ActionNode
{
    [SerializeField][SerializeProperty("Subtree")] BehaviorTree subtree;
    [HideInInspector][SerializeField] public List<bool> passValue = new();
    [SerializeField] bool autoRemapOnAssign = false;

    BehaviorTree runtimeTree;

    public BehaviorTree Subtree
    {
        get
        {
            return subtree;
        }
        set
        {
            if(value != subtree)
            {
                subtree = value;
                checkProperties();

                if(autoRemapOnAssign)
                {
                    autoRemap();
                }
            }
        }
    }

    public SubtreeNode() : base(MemoryMode.Memoried)
    {
    }

    public override void OnStart()
    {
        if(!subtree)
        {
            return;
        }

        if(runtimeTree == null)
        {
            runtimeTree = subtree.Clone();
            runtimeTree.Bind(gameObject);
        }
    }

    public override void OnStop()
    {
    }

    public override NodeState OnUpdate()
    {
        if(!subtree)
        {
            return NodeState.Success;
        }

        for (int i = 0; i < runtimeTree.blackboard.Count; i++)
        {
            if(passValue[i])
            {
                BlackboardProperty property = runtimeTree.blackboard[i];
                property.Value = GetPropertyValue<object>(property.PropertyName);
            }
            
        }

        return runtimeTree.Update();
    }

    void checkProperties()
    {
        if(!subtree)
        {
            ClearPropertyDefinitions();
            return;
        }

        foreach (BlackboardProperty property in subtree.blackboard)
        {
            if(! HasProperty(property.PropertyName))
            {
                CreateProperty(property.GetType(), property.PropertyName);
                passValue.Add(false);
            }
        }

        for (int i = propertyBlackboardMap.Count - 1; i >= 0; i--)
        {
            NameMap map = propertyBlackboardMap[i];
            if (!subtree.blackboard.Any(x => x.PropertyName == map.variable))
            {
                propertyBlackboardMap.RemoveAt(i);
                variables.RemoveAt(i);
                passValue.RemoveAt(i);
            }
        }
    }

    void OnValidate()
    {
        checkProperties();
    }

    public void autoRemap()
    {
        for (int i = 0; i < propertyBlackboardMap.Count; i++)
        {
            NameMap myProperty = propertyBlackboardMap[i];
            foreach (BlackboardProperty bbProperty in tree.blackboard)
            {
                if (myProperty.variable == bbProperty.name)
                {
                    propertyBlackboardMap[i] = new() { variable = myProperty.variable, blackboardProperty = bbProperty.name };
                }
            }
        }
    }

}