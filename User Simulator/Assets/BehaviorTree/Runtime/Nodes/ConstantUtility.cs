using UnityEngine;

public class ConstantUtility : DecoratorNode
{

    public override void OnStart()
    {
    }

    public override void OnStop()
    {
    }

    public override NodeState OnUpdate()
    {
        return child.Update();
    }

    public override void OnCreateProperties()
    {
        CreateProperty(typeof(FloatBlackboardProperty), "value");
    }

    protected override float OnComputeUtility()
    {
        return GetPropertyValue<float>("value");
    }
}