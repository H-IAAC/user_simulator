using UnityEngine;
using HIAAC.BehaviorTrees;
using HIAAC.ScriptableList;

public class ReadListNode : ActionNode
{
    uint currentIndex;
    ScriptableList list;

    public ReadListNode() : base(MemoryMode.Memoried)
    {
        CreateProperty(typeof(ScriptableListBlackboardProperty), "list");
        CreateProperty(typeof(ObjectBlackboardProperty), "output");
        CreateProperty(typeof(UIntBlackboardProperty), "startIndex");
    }

    public override void OnStart()
    {
        list = GetPropertyValue<ScriptableList>("list");

        if(list == null)
        {
            return;
        }

        currentIndex = GetPropertyValue<uint>("startIndex");
    }

    public override void OnStop()
    {
    }

    public override NodeState OnUpdate()
    {
        if(list == null)
        {
            return NodeState.Failure;
        }

        object output = list.getAt((int)currentIndex);

        SetPropertyValue("output", output);

        currentIndex += 1;

        if(currentIndex >= list.Count)
        {
            return NodeState.Success;
        }

        return NodeState.Runnning;
    }

}