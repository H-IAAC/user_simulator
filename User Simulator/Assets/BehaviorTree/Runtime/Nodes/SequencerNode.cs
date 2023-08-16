using UnityEngine;

namespace HIAAC.BehaviorTree
{
    public class SequencerNode : CompositeNode
    {
        Node currentChild;

        public SequencerNode() : base(MemoryMode.Both)
        {

        }

        public override void OnStart()
        {
            currentChild = NextChild();
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

        NodeState memoriedUpdate()
        {
            if (currentChild == null)
            {
                return NodeState.Success;
            }

            NodeState childState = currentChild.Update();

            switch (childState)
            {
                case NodeState.Runnning:
                    return NodeState.Runnning;
                case NodeState.Failure:
                    return NodeState.Failure;
                case NodeState.Success:
                    currentChild = NextChild();
                    return memoriedUpdate();
            }

            return NodeState.Success;
        }

        NodeState memorylessUpdate()
        {
            while (currentChild != null)
            {
                NodeState state = currentChild.Update();

                if (state != NodeState.Success)
                {
                    return state;
                }

                currentChild = NextChild();
            }

            return NodeState.Success;
        }

    }
}