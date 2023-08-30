using UnityEngine;

namespace HIAAC.BehaviorTree
{
    public class ConstantUtility : DecoratorNode
    {
        public ConstantUtility()
        {
            CreateProperty(typeof(FloatBlackboardProperty), "value");
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
            return GetPropertyValue<float>("value");
        }
    }
}