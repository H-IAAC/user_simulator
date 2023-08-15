using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SubtreeNode : ActionNode
{
    [SerializeField][SerializeProperty("Subtree")] protected BehaviorTree subtree;
    [HideInInspector][SerializeField] public List<bool> passValue = new();
    [SerializeField] bool autoRemapOnAssign = false;

    [HideInInspector][SerializeField] protected List<string> propertiesDontDeleteOnValidate = new();

    BehaviorTree runtimeTree;

    public virtual BehaviorTree Subtree
    {
        get
        {
            return subtree;
        }
        set
        {
            if(runtimeTree != null)
            {
                runtimeTree = null;
            }


            if(value != subtree)
            {
                subtree = value;
                ValidateSubtree();
            }
        }
    }

    protected void ValidateSubtree()
    {
        checkProperties();

        if(autoRemapOnAssign)
        {
            autoRemap();
        }
    }

    public BehaviorTree RuntimeTree
    {
        get
        {
            return runtimeTree;
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
            runtimeTree.Start();
        }
    }

    public override void OnStop()
    {
    }

    public override NodeState OnUpdate()
    {
        if(!runtimeTree)
        {
            return NodeState.Failure;
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
            ClearPropertyDefinitions(propertiesDontDeleteOnValidate);
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

            if(propertiesDontDeleteOnValidate.Contains(map.variable))
            {
                continue;
            }

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
                    passValue[i] = true;
                }
            }
        }
    }

    protected override float OnComputeUtility()
    {
        if(!subtree)
        {
            return 0f;
        }

        if(runtimeTree == null)
        {
            runtimeTree = subtree.Clone();
            runtimeTree.Bind(gameObject);

            runtimeTree.Start();
        }

        return runtimeTree.GetUtility();
    }

}