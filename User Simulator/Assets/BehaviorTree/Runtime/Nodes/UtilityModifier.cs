using UnityEngine;

namespace HIAAC.BehaviorTree
{
    public class UtilityModifier : DecoratorNode
    {
        public UtilityModifier()
        {
            CreateProperty(typeof(CurveBlackboardProperty), "modifierFunction");
        }

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

        protected override float OnComputeUtility()
        {
            child.ComputeUtility();
            float childUtility = child.GetUtility();

            AnimationCurve curve = GetPropertyValue<AnimationCurve>("modifierFunction");
            return curve.Evaluate(childUtility);
        }
    }
}