using UnityEngine;

public class UtilityModifier : DecoratorNode
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
        CreateProperty(typeof(CurveBlackboardProperty), "modifierFunction");
    }

    public override float GetUtility()
    {
        float childUtility = child.GetUtility();

        AnimationCurve curve = GetPropertyValue<AnimationCurve>("modifierFunction");
        return curve.Evaluate(childUtility);
    }
}