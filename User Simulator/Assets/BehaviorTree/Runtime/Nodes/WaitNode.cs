using UnityEngine;

public class WaitNode : ActionNode
{
    float startTime;

    public WaitNode() : base(MemoryMode.Memoried)
    {
    }

    public override void OnCreateProperties()
    {
        CreateProperty(typeof(FloatBlackboardProperty), "duration");
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
        float duration = GetPropertyValue<float>("duration");
        if(Time.time - startTime >= duration)
        {
            return NodeState.Success;
        }

        return NodeState.Runnning;
    }
}