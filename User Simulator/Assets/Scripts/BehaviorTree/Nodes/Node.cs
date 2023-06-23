using System.Collections.Generic;
using UnityEngine;

public abstract class Node: ScriptableObject
{
    [HideInInspector] public NodeState state = NodeState.Runnning;
    [HideInInspector] public bool started = false;
    [HideInInspector] public string guid;
    [HideInInspector] public Vector2 position;

    [HideInInspector] public Blackboard blackboard;
    [HideInInspector] public GameObject gameObject;

    [SerializeField][SerializeProperty("UseMemory")]
    bool useMemory = false;
    [TextArea] public string description;
    
    public Node(MemoryMode memoryMode = MemoryMode.Memoryless)
    {
        this.MemoryMode = memoryMode;

        switch (memoryMode)
        {
            case MemoryMode.Memoried:
                this.useMemory = true;
                break;
            case MemoryMode.Memoryless:
                this.useMemory = false;
                break;
            case MemoryMode.Both:
                break;
        }
    }

    public MemoryMode MemoryMode
    {
        private set; get;
    }

    public bool UseMemory
    {
        get
        {
            return useMemory;
        }

        set
        {
            if(MemoryMode == MemoryMode.Both)
            {
                useMemory = value;
            }

        }
    }

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

    public abstract void AddChild(Node child);

    public abstract void RemoveChild(Node child);

    public abstract List<Node> GetChildren();
}