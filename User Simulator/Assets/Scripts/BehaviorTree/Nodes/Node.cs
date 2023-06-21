using UnityEngine;

public abstract class Node: ScriptableObject
{
    [HideInInspector] public NodeState state = NodeState.Runnning;
    [HideInInspector] public bool started = false;
    [HideInInspector] public string guid;
    [HideInInspector] public Vector2 position;

    [HideInInspector] public Blackboard blackboard;
    [HideInInspector] public GameObject gameObject;

    [TextArea] public string description;

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

    public virtual Node Clone()
    {
        return Instantiate(this);
    }

    public abstract void OnStart();
    public abstract void OnStop();
    public abstract NodeState OnUpdate();
}