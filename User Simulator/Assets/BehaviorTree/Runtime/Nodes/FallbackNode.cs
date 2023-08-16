namespace HIAAC.BehaviorTree
{
    public class FallbackNode : CompositeNode
    {
        Node currentChild;

        public FallbackNode() : base(MemoryMode.Both)
        {

        }

        public override void OnStart()
        {
        }

        public override void OnStop()
        {

        }

        public override NodeState OnUpdate()
        {
            if (!UseMemory)
            {
                ResetNext();
                currentChild = NextChild();
            }
            else if (currentChild == null)
            {
                currentChild = NextChild();
            }


            while (currentChild != null)
            {
                NodeState state = currentChild.Update();

                switch (state)
                {
                    case NodeState.Runnning:
                        return NodeState.Runnning;
                    case NodeState.Failure:
                        currentChild = NextChild();
                        break;
                    case NodeState.Success:
                        return NodeState.Success;
                }

            }

            return NodeState.Failure;
        }
    }
}