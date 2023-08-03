using UnityEngine;

public class DebugLogNode : ActionNode
{

    public override void OnCreateProperties()
    {
        CreateProperty(typeof(StringBlackboardProperty), "message");
    }

    public override void OnStart()
    {
        string message = GetPropertyValue<string>("message");
        Debug.Log($"OnStart {message}");
    }

    public override void OnStop()
    {
        string message = GetPropertyValue<string>("message");
        Debug.Log($"OnStop {message}");
    }

    public override NodeState OnUpdate()
    {
        string message = GetPropertyValue<string>("message");
        Debug.Log($"OnUpdate {message}");

        return NodeState.Success;
    }
}