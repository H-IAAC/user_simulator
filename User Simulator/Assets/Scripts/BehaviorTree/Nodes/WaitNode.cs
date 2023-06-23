using UnityEngine;

public class WaitNode : ActionNode
{
    public float duration = 1;
    float startTime;

    public WaitNode() : base(MemoryMode.Memoried)
    {
        
    }

    public override void OnStart()
    {
        startTime = Time.time;
    }

    public override void OnStop()
    {
        
    }

    public override NodeState OnUpdate()
    {
        if(Time.time - startTime >= duration)
        {
            return NodeState.Success;
        }

        return NodeState.Runnning;
    }
}