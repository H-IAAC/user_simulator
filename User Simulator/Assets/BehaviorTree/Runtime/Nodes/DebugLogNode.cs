using UnityEngine;

namespace HIAAC.BehaviorTree
{
    public class DebugLogNode : ActionNode
    {
        [SerializeField] bool onStart = false;
        [SerializeField] bool onStop = false;
        [SerializeField] bool onUpdate = true;

        public DebugLogNode()
        {
            CreateProperty(typeof(StringBlackboardProperty), "message");
        }

        public override void OnStart()
        {
            if (!onStart)
            {
                return;
            }

            string message = GetPropertyValue<string>("message");
            Debug.Log($"OnStart {message}");
        }

        public override void OnStop()
        {
            if (!onStop)
            {
                return;
            }

            string message = GetPropertyValue<string>("message");
            Debug.Log($"OnStop {message}");
        }

        public override NodeState OnUpdate()
        {
            if (!onUpdate)
            {
                return NodeState.Success;
            }

            string message = GetPropertyValue<string>("message");
            Debug.Log($"OnUpdate {message}");

            return NodeState.Success;
        }
    }
}