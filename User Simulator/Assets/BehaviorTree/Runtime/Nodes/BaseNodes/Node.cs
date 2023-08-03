using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public struct NameMap
{
    public string variable;
    public string blackboardProperty;
}

public abstract class Node: ScriptableObject
{
    [HideInInspector] public NodeState state = NodeState.Runnning;
    [HideInInspector] public bool started = false;
    [HideInInspector] public string guid;
    [HideInInspector] public Vector2 position;
    [HideInInspector] public GameObject gameObject;

    [SerializeField][SerializeProperty("UseMemory")]
    bool useMemory = false;
    [TextArea] public string description;

    [HideInInspector] public Node parent;

    [HideInInspector] public List<NameMap> propertyBlackboardMap = new();
    [HideInInspector] public List<BlackboardProperty> variables = new();

    [HideInInspector] public List<BlackboardProperty> blackboard;

    [HideInInspector] public BehaviorTree tree;

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


    public virtual void OnCreateProperties()
    {
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

    public void CreateProperty(Type type, string name)
    {
        propertyBlackboardMap.Add(new NameMap { variable = name, blackboardProperty = "" });

        BlackboardProperty property = ScriptableObject.CreateInstance(type) as BlackboardProperty;
        property.name = this.name + "-" + name;

        #if UNITY_EDITOR
        if(AssetDatabase.GetAssetPath(this) == "")
        {
            throw new Exception("Creating property before object initalization.");
        }

        if (!Application.isPlaying)
            {
                AssetDatabase.AddObjectToAsset(property, AssetDatabase.GetAssetPath(this));
            }
        #endif

        property.PropertyName = name;

        variables.Add(property);
    }

    protected T GetPropertyValue<T>(string name)
    {
        int index = propertyBlackboardMap.FindIndex(x => x.variable == name);

        if(index < 0)
        {
            throw new ArgumentException("Property does not exist in node.");
        }

        string bbName = propertyBlackboardMap[index].blackboardProperty;
        if(bbName == "")
        {
            return (T)variables[index].Value;
        }
        else
        {
            index = blackboard.FindIndex(x => x.PropertyName == bbName);
            if (index < 0)
            {
                throw new ArgumentException($"Property does not exist in blackboard. Property name: {bbName}");
            }

            return (T)blackboard[index].Value;
        }
    }

    public void ClearPropertyDefinitions()
    {
        variables.Clear();
        propertyBlackboardMap.Clear();
    }
    
    public bool HasProperty(string name)
    {
        return propertyBlackboardMap.Any(x => x.variable == name);
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