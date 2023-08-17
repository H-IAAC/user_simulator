using System.Collections.Generic;
using UnityEngine;

namespace HIAAC.BehaviorTree
{
    public abstract class DecoratorNode : Node
    {
        [HideInInspector] public Node child;

        public DecoratorNode(MemoryMode memoryMode = MemoryMode.Memoryless) : base(memoryMode)
        {

        }

        public override Node Clone()
        {
            DecoratorNode node = Instantiate(this);
            node.child = child.Clone();
            node.guid = guid;
            return node;
        }

        public override void AddChild(Node child)
        {
            this.child = child;
            child.parent = this;
        }

        public override void RemoveChild(Node child)
        {
            this.child.parent = null;
            this.child = null;
        }

        public override List<Node> GetChildren()
        {
            List<Node> children = new();

            if (child != null)
            {
                children.Add(child);
            }

            return children;
        }

        protected override float OnComputeUtility()
        {
            child.ComputeUtility();
            return child.GetUtility();
        }
    }
}