using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace HIAAC.BehaviorTree
{
    [CreateAssetMenu(menuName = "Behavior Tree/Behavior Tree")]
    public class BehaviorTree : ScriptableObject
    {
        public RootNode rootNode;
        public NodeState treeState = NodeState.Runnning;

        public List<Node> nodes = new();

        public List<BlackboardProperty> blackboard = new();

        public List<BTagParameter> bTagParameters = new();

        [HideInInspector] public bool runtime = false;

        public NodeState Update()
        {
            if (rootNode.state == NodeState.Runnning)
            {
                treeState = rootNode.Update();
            }

            return treeState;
        }

        public Node CreateNode(Type type)
        {
            Node node = CreateInstance(type) as Node;
            node.name = type.Name;
            node.blackboard = this.blackboard;
            node.tree = this;



#if UNITY_EDITOR
            Undo.RecordObject(this, "Behavior Tree (CreateNode)");
#endif

            nodes.Add(node);

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                AssetDatabase.AddObjectToAsset(node, this);
            }
            Undo.RegisterCreatedObjectUndo(node, "Behavior Tree (CreateNode)");

            AssetDatabase.SaveAssets();
#endif

            node.OnCreateProperties();

            AssetDatabase.SaveAssets();

            return node;
        }

        public Node DuplicateNode(Node node)
        {
            if (!nodes.Contains(node))
            {
                throw new ArgumentException("Node to duplicate is not in tree.");
            }

            Node clone = CreateNode(node.GetType());
            clone.position = node.position;
            clone.position.x += 10;
            clone.UseMemory = node.UseMemory;
            clone.description = node.description;

            foreach (BlackboardProperty var in clone.variables)
            {
                var.Value = node.GetPropertyValue(var.PropertyName, true);
            }

            for (int i = 0; i < node.propertyBlackboardMap.Count; i++)
            {
                NameMap map = node.propertyBlackboardMap[i];
                NameMap cloneMap = new()
                {
                    blackboardProperty = map.blackboardProperty,
                    variable = map.variable
                };

                clone.propertyBlackboardMap[i] = cloneMap;
            }

            if (clone is CompositeNode compositeClone)
            {
                CompositeNode composite = node as CompositeNode;
                compositeClone.useUtility = composite.useUtility;
            }

            return clone;
        }

        public void DeleteNode(Node node)
        {
#if UNITY_EDITOR
            Undo.RecordObject(this, "Behavior Tree (DeleteNode)");
#endif

            nodes.Remove(node);
            node.ClearPropertyDefinitions();

#if UNITY_EDITOR
            //AssetDatabase.RemoveObjectFromAsset(node);
            Undo.DestroyObjectImmediate(node);

            AssetDatabase.SaveAssets();
#endif
        }

        public void DeleteProperty(BlackboardProperty property)
        {
#if UNITY_EDITOR
            Undo.RecordObject(this, "Behavior Tree (DeleteTreeProperty)");
#endif

            blackboard.Remove(property);

#if UNITY_EDITOR
            //AssetDatabase.RemoveObjectFromAsset(node);
            Undo.DestroyObjectImmediate(property);

            AssetDatabase.SaveAssets();
#endif
        }


        public void Traverse(Node node, Action<Node> visiter)
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

        public BehaviorTree Clone()
        {
            BehaviorTree tree = Instantiate(this);
            tree.rootNode = tree.rootNode.Clone() as RootNode;

            tree.nodes = new List<Node>();
            List<string> clonedGUID = new();

            Traverse(tree.rootNode, (node) => { tree.nodes.Add(node); clonedGUID.Add(node.guid); });

            foreach (Node origNode in this.nodes)
            {
                if (!clonedGUID.Contains(origNode.guid))
                {
                    Node parent = origNode;
                    while (parent.parent != null)
                    {
                        parent = parent.parent;
                    }

                    Node cloned = parent.Clone();

                    Traverse(cloned, (node) => { tree.nodes.Add(node); clonedGUID.Add(node.guid); });
                }
            }

            tree.blackboard = new();
            foreach (BlackboardProperty property in blackboard)
            {
                tree.blackboard.Add(property.Clone());
            }
            foreach (Node node in tree.nodes)
            {
                node.blackboard = tree.blackboard;
                node.tree = tree;
            }

            return tree;
        }

        public void Bind(GameObject gameObject)
        {
            runtime = true;
            Traverse(rootNode, (node) =>
                {
                    node.gameObject = gameObject;
                }
            );
        }

        public void Validate()
        {
            nodes.RemoveAll(item => item == null);
            blackboard.RemoveAll(item => item == null);

            nodes.ForEach(x => x.tree = this);
        }

        public float GetUtility()
        {
            return rootNode.GetUtility();
        }

        public void Start()
        {
            if (!rootNode.started)
            {
                rootNode.Start();
            }
        }
    }
}