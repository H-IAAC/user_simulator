using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HIAAC.BehaviorTree
{
    [Serializable]
    public struct NameMap
    {
        public string variable;
        public string blackboardProperty;
    }

    public abstract class Node : ScriptableObject
    {
        [HideInInspector] public NodeState state = NodeState.Runnning;
        [HideInInspector] public bool started = false;
        [HideInInspector] public string guid;
        [HideInInspector] public Vector2 position;
        [HideInInspector] public GameObject gameObject;

        [SerializeField]
        [SerializeProperty("UseMemory")]
        bool useMemory = false;
        [TextArea] public string description;

        [HideInInspector] public Node parent;

        [HideInInspector] public List<NameMap> propertyBlackboardMap = new();
        [HideInInspector] public List<BlackboardProperty> variables = new();

        [HideInInspector] public List<BlackboardProperty> blackboard;

        [HideInInspector] public BehaviorTree tree;

        float utility = 0f;

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

            guid = Guid.NewGuid().ToString();
        }

        void Awake()
        {
            guid = Guid.NewGuid().ToString();
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
                if (MemoryMode == MemoryMode.Both)
                {
                    useMemory = value;
                }

            }
        }

        public NodeState Update()
        {
            if (!started)
            {
                Start();
                started = true;
            }

            state = OnUpdate();

            if (state == NodeState.Failure || state == NodeState.Success)
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
            if (gameObject == null)
            {
                if (AssetDatabase.GetAssetPath(this) == "")
                {
                    throw new Exception("Creating property before object initalization.");
                }

                if (!Application.isPlaying)
                {
                    AssetDatabase.AddObjectToAsset(property, AssetDatabase.GetAssetPath(this));
                }
            }
#endif

            property.PropertyName = name;

            variables.Add(property);
        }

        private BlackboardProperty GetProperty(string name, bool forceNodeProperty = false)
        {
            int index = propertyBlackboardMap.FindIndex(x => x.variable == name);

            if (index < 0)
            {
                throw new ArgumentException("Property does not exist in node.");
            }

            string bbName = propertyBlackboardMap[index].blackboardProperty;
            if (bbName == "" || forceNodeProperty)
            {
                return variables[index];
            }
            else
            {
                index = blackboard.FindIndex(x => x.PropertyName == bbName);
                if (index < 0)
                {
                    throw new ArgumentException($"Property does not exist in blackboard. Property name: {bbName}");
                }

                return blackboard[index];
            }
        }

        public object GetPropertyValue(string name, bool forceNodeProperty = false)
        {
            return GetProperty(name, forceNodeProperty).Value;
        }

        public T GetPropertyValue<T>(string name, bool forceNodeProperty = false)
        {

            return (T)GetProperty(name, forceNodeProperty).Value;
        }

        public void SetPropertyValue<T>(string name, T value, bool forceNodeProperty = false)
        {
            GetProperty(name, forceNodeProperty).Value = value;
        }

        public void ClearPropertyDefinitions(List<string> dontDelete = null)
        {

#if UNITY_EDITOR

            foreach (BlackboardProperty variable in variables)
            {
                if (variable != null)
                {
                    if (dontDelete != null && !dontDelete.Contains(variable.name))
                    {
                        AssetDatabase.RemoveObjectFromAsset(variable);
                    }

                }
            }

#endif

            if (dontDelete != null)
            {
                variables.RemoveAll(x => !dontDelete.Contains(x.name));
                propertyBlackboardMap.RemoveAll(x => !dontDelete.Contains(x.variable));
            }
            else
            {
                variables.Clear();
                propertyBlackboardMap.Clear();
            }

        }

        public bool HasProperty(string name)
        {
            return propertyBlackboardMap.Any(x => x.variable == name);
        }

        public virtual Node Clone()
        {
            Node clone = Instantiate(this);
            clone.guid = guid;
            return clone;
        }

        public float GetUtility()
        {
            return utility;
        }

        public void ComputeUtility()
        {
            utility = OnComputeUtility();
        }

        protected virtual float OnComputeUtility()
        {
            return 0f;
        }

        public void Start()
        {
            ComputeUtility();
            OnStart();
        }

        /// <summary>
        /// Tranverse the node and all children applying some action.
        /// </summary>
        /// <param name="node">Start node to visit.</param>
        /// <param name="visiter">Action to aply on the nodes.</param>
        public static void Traverse(Node node, Action<Node> visiter)
        {
            if (node)
            {
                visiter.Invoke(node);
                List<Node> children = node.GetChildren();

                foreach (Node child in children)
                {
                    Traverse(child, visiter);
                }
            }
        }

        public abstract void OnStart();
        public abstract void OnStop();
        public abstract NodeState OnUpdate();

        public abstract void AddChild(Node child); //Must set child parent

        public abstract void RemoveChild(Node child); //Must set child parent

        public abstract List<Node> GetChildren();
    }
}