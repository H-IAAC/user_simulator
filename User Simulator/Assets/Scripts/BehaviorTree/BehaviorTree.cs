using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

[CreateAssetMenu(menuName="Behavior Tree/Behavior Tree")]
public class BehaviorTree: ScriptableObject
{
    public RootNode rootNode;
    public NodeState treeState = NodeState.Runnning;

    public List<Node> nodes = new();


    public Blackboard blackboard = new();

    [HideInInspector] public bool runtime = false;

    public NodeState Update()
    {
        if(rootNode.state == NodeState.Runnning)
        {
            treeState = rootNode.Update();
        }

        return treeState;
    }

    public Node CreateNode(Type type)
    {
        Node node = CreateInstance(type) as Node;
        node.name = type.Name;

        node.guid = Guid.NewGuid().ToString();

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

        

        return node;
    }

    public void DeleteNode(Node node)
    {
        #if UNITY_EDITOR
            Undo.RecordObject(this, "Behavior Tree (DeleteNode)");
        #endif

        nodes.Remove(node);
        
        #if UNITY_EDITOR
            //AssetDatabase.RemoveObjectFromAsset(node);
            Undo.DestroyObjectImmediate(node);

            AssetDatabase.SaveAssets();
        #endif
    }


    public void Traverse(Node node, Action<Node> visiter)
    {
        if(node)
        {
            visiter.Invoke(node);
            List<Node> children = node.GetChildren();

            foreach(Node child in children)
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

        foreach(Node origNode in this.nodes)
        {
            if(!clonedGUID.Contains(origNode.guid))
            {
                Node parent = origNode;
                while(parent.parent != null)
                {
                    parent = parent.parent;
                }

                Node cloned = parent.Clone();

                Traverse(cloned, (node) => { tree.nodes.Add(node); clonedGUID.Add(node.guid); });
            }
        }

        return tree;
    }

    public void Bind(GameObject gameObject)
    {
        runtime = true;
        Traverse(rootNode, (node) =>
            {
                node.gameObject = gameObject;
                node.blackboard = blackboard;
            }
        );
    }
}