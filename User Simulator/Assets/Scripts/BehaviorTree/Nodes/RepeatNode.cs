using UnityEngine;

public class RepeatNode: DecoratorNode
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