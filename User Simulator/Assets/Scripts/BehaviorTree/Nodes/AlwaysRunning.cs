using UnityEngine;

public class AlwaysRunning : DecoratorNode
{

    public override void OnStart()
    {
    }

    public override void OnStop()
    {
        
    }

    public override NodeState OnUpdate()
    {
        child.Update();

        return NodeState.Runnning;
    }
}