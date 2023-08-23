using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HIAAC.BehaviorTree
{
    /// <summary>
    /// Main Behavior Tree class.
    /// </summary>
    [CreateAssetMenu(menuName = "Behavior Tree/Behavior Tree")]
    public class BehaviorTree : ScriptableObject
    {
        NodeState treeState = NodeState.Runnning; //Current tree state (same as root)

        public RootNode rootNode; //Root node 
        public List<Node> nodes = new(); //All nodes

        public List<BlackboardProperty> blackboard = new(); //Blackboard proprieties of the tree.

        public List<BTagParameter> bTagParameters = new(); //BehaviorTags of the tree.

        bool runtime = false; //If the tree is runtime (is binded to object and can be runned).

        /// <summary>
        /// If the tree is runtime (is binded to object and can be runned).
        /// </summary>
        public bool Runtime
        {
            get
            {
                return runtime;
            }
        }

        // Lifecyle // ----------------------------------------------------------------------------------- //

        /// <summary>
        /// Binds the tree to the game object. 
        /// All nodes will have the refence to the object.
        /// </summary>
        /// <param name="gameObject">GameObject to bind the tree.</param>
        public void Bind(GameObject gameObject)
        {
            runtime = true;

            Node.Traverse(rootNode, (node) =>
                {
                    node.gameObject = gameObject;
                }
            );
        }

        /// <summary>
        /// Start the tree.
        /// 
        /// Is automatically done when calling Update for the first time.
        /// </summary>
        /// <exception cref="Exception">If the tree is not runtime (can't be run).</exception>
        public void Start()
        {
            if(!runtime)
            {
                throw new Exception("Trying to run non-runtime tree");
            }

            if (!rootNode.started)
            {
                rootNode.Start();
            }
        }

        /// <summary>
        /// Updates the tree, tranversing it's nodes.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception">If the tree is not runtime (can't be run).</exception>
        public NodeState Update()
        {
            if(!runtime)
            {
                throw new Exception("Trying to run non-runtime tree");
            }

            if (rootNode.state == NodeState.Runnning)
            {
                treeState = rootNode.Update();
            }

            return treeState;
        }

        /// <summary>
        /// Get the tree utility value (same as the root node).
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception">If the tree is not runtime (doesn't have utility value).</exception>
        public float GetUtility()
        {
            if(!runtime)
            {
                throw new Exception("Trying to run non-runtime tree");
            }
            
            return rootNode.GetUtility();
        }
        
        // Tree manipulation // -------------------------------------------------------------------------- //


        public BehaviorTree Clone()
        {
            //Create new tree and clone nodes.
            BehaviorTree tree = Instantiate(this);
            tree.rootNode = tree.rootNode.Clone() as RootNode;

            //Add nodes to tree and create new-original map.
            tree.nodes = new List<Node>();
            List<string> clonedGUID = new();
            Node.Traverse(tree.rootNode, (node) => { tree.nodes.Add(node); clonedGUID.Add(node.guid); });

            //Check for nodes that aren't in main tree (not child of root) and clone.
            foreach (Node origNode in this.nodes)
            {
                if (!clonedGUID.Contains(origNode.guid)) //Node not cloned
                {
                    //Get root node of other tree
                    Node parent = origNode;
                    while (parent.parent != null)
                    {
                        parent = parent.parent;
                    }

                    //Clone nodes and add to the tree
                    Node cloned = parent.Clone();
                    Node.Traverse(cloned, (node) => { tree.nodes.Add(node); clonedGUID.Add(node.guid); });
                }
            }

            //Clone blackboard properties
            tree.blackboard = new();
            foreach (BlackboardProperty property in blackboard)
            {
                tree.blackboard.Add(property.Clone());
            }

            //Assign blackboard and tree to nodes
            foreach (Node node in tree.nodes)
            {
                node.blackboard = tree.blackboard;
                node.tree = tree;
            }

            return tree;
        }

        public void Validate()
        {
            nodes.RemoveAll(item => item == null);
            blackboard.RemoveAll(item => item == null);

            nodes.ForEach(x => x.tree = this);

            if (blackboard == null)
            {
                blackboard = new();
            }

        }

        // Properties // --------------------------------------------------------------------------------- //

        public BlackboardProperty CreateProperty(Type type)
        {
            BlackboardProperty property = ScriptableObject.CreateInstance(type) as BlackboardProperty;

            //Ensures name isn't duplicated
            string name = property.PropertyTypeName;
            while (blackboard.Any(x => x.PropertyName == name))
            {
                name += "(1)";
            }
            property.PropertyName = name;

            //Add property
            blackboard.Add(property);
            if (!Application.isPlaying)
            {
                AssetDatabase.AddObjectToAsset(property, this);
            }

            //Save property to asset
#if UNITY_EDITOR
            Undo.RegisterCreatedObjectUndo(property, "Behavior Tree (CreateProperty)");
            AssetDatabase.SaveAssets();
#endif

            return property;
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


        // Nodes // -------------------------------------------------------------------------------------- //

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
    }
}