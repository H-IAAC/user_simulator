using UnityEngine;

public class RepeatNode: DecoratorNode
{
    public int repeatCount = 1;
    int currentRepeatCount = 0;
    
    void OnEnable()
    {
        currentRepeatCount = 0;
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

        if(currentRepeatCount >= repeatCount)
        {
            currentRepeatCount = 0;
            return NodeState.Success;
        }

        return NodeState.Runnning;
    }
}