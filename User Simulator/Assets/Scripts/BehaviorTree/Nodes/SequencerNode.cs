using UnityEngine;

public class SequencerNode: CompositeNode
{
    int current = 0;

    void OnEnable()
    {
        current = 0;
    }

    public override void OnStart()
    {

    }

    public override void OnStop()
    {

    }

    public override NodeState OnUpdate()
    {
        if(useMemory)
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
        if(current >= children.Count)
        {
            current = 0;
            return NodeState.Success;
        }

        NodeState childState = children[current].Update();

        switch (childState)
        {
            case NodeState.Runnning:
                return NodeState.Runnning;
            case NodeState.Failure:
                return NodeState.Failure;
            case NodeState.Success:
                current++;
                return memoriedUpdate();
        }

        return NodeState.Success;
    }

    NodeState memorylessUpdate()
    {
        foreach(Node child in children)
        {
            NodeState state = child.Update();

            if(state != NodeState.Success)
            {
                return state;
            }
        }

        return NodeState.Success;
    }

}