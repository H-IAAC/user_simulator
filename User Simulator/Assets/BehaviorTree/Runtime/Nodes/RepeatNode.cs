using UnityEngine;

namespace HIAAC.BehaviorTree
{
    public class RepeatNode : DecoratorNode
    {
        uint repeatCount;
        int currentRepeatCount = 0;

        public RepeatNode() : base(MemoryMode.Both)
        {

        }

        public override void OnCreateProperties()
        {
            CreateProperty(typeof(UIntBlackboardProperty), "repeatCount");
            SetPropertyValue<uint>("repeatCount", 1);
        }


        public override void OnStart()
        {
            currentRepeatCount = 0;
            repeatCount = GetPropertyValue<uint>("repeatCount");
        }

        public override void OnStop()
        {

        }

        public override NodeState OnUpdate()
        {
            if (UseMemory)
            {
                return memoriedUpdate();
            }
            else
            {
                return memorylessUpdate();
            }
        }

        NodeState memorylessUpdate()
        {
            for (int i = 0; i < repeatCount; i++)
            {
                NodeState state = child.Update();

                switch (state)
                {
                    case NodeState.Runnning:
                        return NodeState.Runnning;
                    case NodeState.Failure:
                        currentRepeatCount += 1;
                        break;
                    case NodeState.Success:
                        currentRepeatCount += 1;
                        break;
                }
            }

            return NodeState.Success;
        }

        NodeState memoriedUpdate()
        {
            NodeState state = child.Update();

            switch (state)
            {
                case NodeState.Runnning:
                    return NodeState.Runnning;
                case NodeState.Failure:
                    currentRepeatCount += 1;
                    break;
                case NodeState.Success:
                    currentRepeatCount += 1;
                    break;
            }

            if (currentRepeatCount >= repeatCount)
            {
                currentRepeatCount = 0;
                return NodeState.Success;
            }

            return NodeState.Runnning;
        }
    }
}