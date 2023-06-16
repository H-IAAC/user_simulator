using UnityEngine;

public class DebugLogNode : ActionNode
{
    public string message;

    public override void OnStart()
    {
        Debug.Log($"OnStart{message}");
    }

    public override void OnStop()
    {
        Debug.Log($"OnStop{message}");
    }

    public override NodeState OnUpdate()
    {
        Debug.Log($"OnUpdate{message}");

        return NodeState.Success;
    }
}