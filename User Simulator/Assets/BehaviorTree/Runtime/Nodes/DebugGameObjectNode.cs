using UnityEngine;

public class DebugGameObjectNode : ActionNode
{

    public override void OnStart()
    {

    }

    public override void OnStop()
    {
    }

    public override NodeState OnUpdate()
    {
        Debug.Log($"GameObject: {gameObject}");

        return NodeState.Success;
    }
}