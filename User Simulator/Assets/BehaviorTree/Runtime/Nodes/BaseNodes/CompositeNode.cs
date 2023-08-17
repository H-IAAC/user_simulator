using System.Collections.Generic;
using UnityEngine;

namespace HIAAC.BehaviorTree
{
    public enum UtilityPropagationMethod
    {
        MAXIMUM,
        MINIMUM,
        ALL_SUCESS_PROBABILITY,
        AT_LEAST_ONE_SUCESS_PROBABILITY,
        SUM,
        AVERAGE
    }

    public enum UtilitySelectionMethod
    {
        MAXIMUM,
        WEIGHT_RANDOM,
        RANDOM_THRESHOULD
    }

    public abstract class CompositeNode : Node
    {
        //[HideInInspector]
        public List<Node> children = new();

        [SerializeField] public bool useUtility = false;
        [SerializeField] public UtilityPropagationMethod utilityPropagationMethod = UtilityPropagationMethod.MAXIMUM;
        [SerializeField] public UtilitySelectionMethod utilitySelectionMethod = UtilitySelectionMethod.MAXIMUM;
        [SerializeField] public float utilityThreshould = 0f;

        public CompositeNode(MemoryMode memoryMode = MemoryMode.Memoryless) : base(memoryMode)
        {

        }

        public override Node Clone()
        {
            CompositeNode node = Instantiate(this);

            node.children = new List<Node>();

            foreach (Node child in children)
            {
                node.children.Add(child.Clone());
            }

            node.guid = guid;

            return node;
        }

        public override void AddChild(Node child)
        {
            children.Add(child);
            child.parent = this;
        }

        public override void RemoveChild(Node child)
        {
            children.Remove(child);
            child.parent = null;
        }

        public override List<Node> GetChildren()
        {
            return children;
        }

        protected void SortChildrenByUtility()
        {
            children.Sort(SortByUtility);
        }

        private int SortByUtility(Node left, Node right)
        {
            if (left.GetUtility() < right.GetUtility())
            {
                return 1;
            }
            return -1;
        }

        protected override float OnComputeUtility()
        {
            foreach (Node child in children)
            {
                child.ComputeUtility();
            }

            UpdateNextChildren();

            switch (utilityPropagationMethod)
            {
                case UtilityPropagationMethod.MAXIMUM:
                    {
                        float biggest = 0;
                        foreach (Node child in children)
                        {
                            float u = child.GetUtility();
                            if (u > biggest)
                            {
                                biggest = u;
                            }
                        }

                        return biggest;
                    }
                case UtilityPropagationMethod.MINIMUM:
                    {
                        float smallest = 1;
                        foreach (Node child in children)
                        {
                            float u = child.GetUtility();
                            if (u < smallest)
                            {
                                smallest = u;
                            }
                        }

                        return smallest;
                    }
                case UtilityPropagationMethod.ALL_SUCESS_PROBABILITY:
                    {
                        float p = 1;
                        foreach (Node child in children)
                        {
                            p *= child.GetUtility();
                        }

                        return p;
                    }
                case UtilityPropagationMethod.AT_LEAST_ONE_SUCESS_PROBABILITY:
                    {
                        if (children.Count == 0)
                        {
                            return 0;
                        }

                        float p = 1;
                        foreach (Node child in children)
                        {
                            p *= 1 - child.GetUtility();
                        }

                        return 1 - p;
                    }
                case UtilityPropagationMethod.SUM:
                    {
                        float utility = 0f;
                        foreach (Node child in children)
                        {
                            utility += child.GetUtility();
                        }

                        return utility;
                    }
                case UtilityPropagationMethod.AVERAGE:
                    {
                        float utility = 0f;
                        foreach (Node child in children)
                        {
                            utility += child.GetUtility();
                        }

                        return utility / children.Count;
                    }
                default:
                    return 0;
            }

        }

        List<Node> nextChildren = new();
        int currentIndex = -1;

        protected void ResetNext()
        {
            currentIndex = -1;
        }

        protected Node NextChild()
        {
            currentIndex += 1;
            if (currentIndex >= nextChildren.Count)
            {
                return null;
            }

            return nextChildren[currentIndex];
        }

        void UpdateNextChildren()
        {
            currentIndex = -1;
            if (!useUtility)
            {
                nextChildren = children;
                return;
            }


            switch (utilitySelectionMethod)
            {

                //sort(a,b)
                //-1: a fica antes de b
                //1: a fica depois de b
                case UtilitySelectionMethod.MAXIMUM:
                    nextChildren = new(children);
                    nextChildren.Sort((node1, node2) =>
                    {
                        if (node1.GetUtility() > node2.GetUtility()) { return -1; }
                        else if (node1.GetUtility() < node2.GetUtility()) { return 1; }
                        return 0;
                    });
                    break;

                case UtilitySelectionMethod.WEIGHT_RANDOM:
                    {
                        nextChildren.Clear();
                        float weightTotal = 0;
                        List<Node> nodes = new();
                        foreach (Node node in children)
                        {
                            nodes.Add(node);
                            weightTotal += node.GetUtility();
                        }

                        nodes.Sort((node1, node2) =>
                        {
                            if (node1.GetUtility() > node2.GetUtility()) { return -1; }
                            else if (node1.GetUtility() < node2.GetUtility()) { return 1; }
                            return 0;
                        });

                        while (nodes.Count > 1)
                        {
                            int result;
                            float total = 0;
                            float randVal = Random.Range(0, weightTotal);
                            for (result = 0; result < nodes.Count; result++)
                            {
                                total += nodes[result].GetUtility();
                                if (total > randVal) break;
                            }


                            Node next = nodes[result];

                            weightTotal -= next.GetUtility();
                            nodes.RemoveAt(result);

                            nextChildren.Add(next);
                        }

                        nextChildren.Add(nodes[0]);

                        break;
                    }
                case UtilitySelectionMethod.RANDOM_THRESHOULD:
                    {
                        nextChildren = new(children);

                        for (int i = children.Count - 1; i >= 0; i--)
                        {
                            if (nextChildren[i].GetUtility() < utilityThreshould)
                            {
                                nextChildren.RemoveAt(i);
                            }
                        }

                        nextChildren.Shuffle();


                        break;
                    }
            }
        }

    }

}