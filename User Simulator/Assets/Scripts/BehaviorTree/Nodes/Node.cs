using UnityEngine;

public abstract class Node: ScriptableObject
{
    public NodeState state = NodeState.Runnning;
    public bool started = false;

    public NodeState Update()
    {
        if(!started)
        {
            OnStart();
            started = true;
        }

        state = OnUpdate();

        if(state == NodeState.Failure || state == NodeState.Success)
        {
            OnStop();
            started = false;
        }

        return state;
    }

    public abstract void OnStart();
    public abstract void OnStop();
    public abstract NodeState OnUpdate();
}