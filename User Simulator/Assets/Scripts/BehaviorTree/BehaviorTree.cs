using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName="Behavior Tree/Behavior Tree")]
public class BehaviorTree: ScriptableObject
{
    public RootNode rootNode;
    public NodeState treeState = NodeState.Runnning;

    public List<Node> nodes = new();


    public Blackboard blackboard = new();

    public NodeState Update()
    {
        if(rootNode.state == NodeState.Runnning)
        {
            treeState = rootNode.Update();
        }

        return treeState;
    }

    public Node CreateNode(System.Type type)
    {
        Node node = ScriptableObject.CreateInstance(type) as Node;
        node.name = type.Name;
        node.guid = GUID.Generate().ToString();

        Undo.RecordObject(this, "Behavior Tree (CreateNode)");

        nodes.Add(node);

        if (!Application.isPlaying)
        {
            AssetDatabase.AddObjectToAsset(node, this);
        }

        

        Undo.RegisterCreatedObjectUndo(node, "Behavior Tree (CreateNode)");

        AssetDatabase.SaveAssets();

        return node;
    }

    public void DeleteNode(Node node)
    {
        Undo.RecordObject(this, "Behavior Tree (DeleteNode)");

        nodes.Remove(node);

        //AssetDatabase.RemoveObjectFromAsset(node);
        Undo.DestroyObjectImmediate(node);

        AssetDatabase.SaveAssets();
    }

    public void AddChild(Node parent, Node child)
    {
        if(parent is DecoratorNode)
        {
            DecoratorNode decorator = parent as DecoratorNode;

            Undo.RecordObject(decorator, "Behavior Tree (AddChild)");

            decorator.child = child;

            EditorUtility.SetDirty(decorator);
        }
        else if (parent is CompositeNode)
        {
            CompositeNode composite = parent as CompositeNode;

            Undo.RecordObject(composite, "Behavior Tree (AddChild)");

            composite.children.Add(child);

            EditorUtility.SetDirty(composite);
        }
        else if(parent is RootNode)
        {
            RootNode root = parent as RootNode;

            Undo.RecordObject(root, "Behavior Tree (AddChild)");

            root.child = child;

            EditorUtility.SetDirty(root);
        }


    }

    public void RemoveChild(Node parent, Node child)
    {
        if(parent is DecoratorNode)
        {
            DecoratorNode decorator = parent as DecoratorNode;

            Undo.RecordObject(decorator, "Behavior Tree (RemoveChild)");

            decorator.child = null;

            EditorUtility.SetDirty(decorator);
        }
        else if (parent is CompositeNode)
        {
            CompositeNode composite = parent as CompositeNode;

            Undo.RecordObject(composite, "Behavior Tree (RemoveChild)");
            
            composite.children.Remove(child);

            EditorUtility.SetDirty(composite);
        }
        else if(parent is RootNode)
        {
            RootNode root = parent as RootNode;

            Undo.RecordObject(root, "Behavior Tree (RemoveChild)");

            root.child = null;

            EditorUtility.SetDirty(root);
        }
    }

    public List<Node> GetChildren(Node parent)
    {

        if(parent is DecoratorNode)
        {
            DecoratorNode decorator = parent as DecoratorNode;

            if(decorator.child != null)
            {   
                Node[] c = {decorator.child};
                return new List<Node>(c);
            }
        }
        else if (parent is CompositeNode)
        {
            CompositeNode composite = parent as CompositeNode;

            return composite.children;
        }
        else if(parent is RootNode)
        {
            RootNode root = parent as RootNode;
            if(root.child != null)
            {   
                Node[] c = {root.child};
                return new List<Node>(c);
            }
        }

        return new List<Node>();
    }

    public void Traverse(Node node, System.Action<Node> visiter)
    {
        if(node)
        {
            visiter.Invoke(node);
            List<Node> children = GetChildren(node);

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
        Traverse(tree.rootNode, (node) => { tree.nodes.Add(node); });

        return tree;
    }

    public void Bind(GameObject gameObject)
    {
        Traverse(rootNode, (node) =>
            {
                node.gameObject = gameObject;
                node.blackboard = blackboard;
            }
        );
    }
}