using UnityEngine;

public class SequencerNode: CompositeNode
{
    public override void OnStart()
    {

    }

    public override void OnStop()
    {

    }

    public override NodeState OnUpdate()
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