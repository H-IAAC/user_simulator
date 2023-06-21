using UnityEngine;

public class FallbackNode: CompositeNode
{
    int current = 0;

    public override void OnStart()
    {

    }

    public override void OnStop()
    {

    }

    public override NodeState OnUpdate()
    {
        if(!useMemory)
        {
            current = 0;
        }

        for (int i = current; i < children.Count; i++)
        {
            NodeState state = children[i].Update();

            switch (state)
            {
                case NodeState.Runnning:
                    current = i;
                    return NodeState.Runnning;
                case NodeState.Failure:
                    break;
                case NodeState.Success:
                    current = 0;
                    return NodeState.Success;
            }

        }

        current = 0;
        return NodeState.Failure;
    }


}