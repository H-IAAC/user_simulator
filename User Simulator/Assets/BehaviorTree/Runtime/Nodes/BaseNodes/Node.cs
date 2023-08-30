using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;
using System.Linq;


namespace HIAAC.BehaviorTree
{
    

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


        [HideInInspector] public BehaviorTree tree;

        [HideInInspector]
        [SerializeField] public Blackboard blackboard;

        float utility = 0f;

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

        // Constructor and lifecycle // --------------------------------------- //

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

            blackboard = new(this, false);
        }

        void Awake()
        {
            guid = Guid.NewGuid().ToString();
        }

        public void Start()
        {
            ComputeUtility();
            OnStart();
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

        // Clone // ----------------------------------------------------------- //

        public virtual Node Clone()
        {
            Node clone = Instantiate(this);
            clone.guid = guid;
            return clone;
        }

        // Utility // --------------------------------------------------------- //

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

        // Properties // ------------------------------------------------------ //

        //TODO: Check name changed duplicate
        public BlackboardProperty CreateProperty(Type type, string name)
        {
            return blackboard.CreateProperty(type, name);
        }

        private BlackboardProperty GetProperty(string name, bool forceNodeProperty = false)
        {
            return blackboard.GetProperty(name, forceNodeProperty);
        }

        public object GetPropertyValue(string name, bool forceNodeProperty = false)
        {
            return blackboard.GetProperty(name, forceNodeProperty).Value;
        }

        public T GetPropertyValue<T>(string name, bool forceNodeProperty = false)
        {
            return blackboard.GetPropertyValue<T>(name, forceNodeProperty);
        }

        public void SetPropertyValue<T>(string name, T value, bool forceNodeProperty = false)
        {
            blackboard.GetProperty(name, forceNodeProperty).Value = value;
        }

        public void ClearPropertyDefinitions(List<string> dontDelete = null)
        {
            blackboard.ClearPropertyDefinitions(dontDelete);
        }

        public bool HasProperty(string name)
        {
            return blackboard.HasProperty(name);
        }


        // To override // ----------------------------------------------------- //
        
        public abstract void OnStart();
        public abstract void OnStop();
        public abstract NodeState OnUpdate();

        public abstract void AddChild(Node child); //Must set child parent

        public abstract void RemoveChild(Node child); //Must set child parent

        public abstract List<Node> GetChildren();
    }
}