using UnityEngine;

namespace HIAAC.BehaviorTree
{
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

        protected override float OnComputeUtility()
        {
            child.ComputeUtility();
            float childUtility = child.GetUtility();

            AnimationCurve curve = GetPropertyValue<AnimationCurve>("modifierFunction");
            return curve.Evaluate(childUtility);
        }
    }
}