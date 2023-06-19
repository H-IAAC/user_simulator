using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName="Behavior Tree/Behavior Tree")]
public class BehaviorTree: ScriptableObject
{
    public RootNode rootNode;
    public NodeState treeState = NodeState.Runnning;

    public List<Node> nodes = new List<Node>();

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
        nodes.Add(node);

        AssetDatabase.AddObjectToAsset(node, this);
        AssetDatabase.SaveAssets();

        return node;
    }

    public void DeleteNode(Node node)
    {
        nodes.Remove(node);

        AssetDatabase.RemoveObjectFromAsset(node);
        AssetDatabase.SaveAssets();
    }

    public void AddChild(Node parent, Node child)
    {
        if(parent is DecoratorNode)
        {
            DecoratorNode decorator = parent as DecoratorNode;

            decorator.child = child;
        }
        else if (parent is CompositeNode)
        {
            CompositeNode composite = parent as CompositeNode;

            composite.children.Add(child);
        }
        else if(parent is RootNode)
        {
            RootNode root = parent as RootNode;
            root.child = child;
        }


    }

    public void RemoveChild(Node parent, Node child)
    {
        if(parent is DecoratorNode)
        {
            DecoratorNode decorator = parent as DecoratorNode;

            decorator.child = null;
        }
        else if (parent is CompositeNode)
        {
            CompositeNode composite = parent as CompositeNode;

            composite.children.Remove(child);
        }
        else if(parent is RootNode)
        {
            RootNode root = parent as RootNode;
            root.child = null;
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

    public BehaviorTree Clone()
    {
        BehaviorTree tree = Instantiate(this);
        tree.rootNode = tree.rootNode.Clone() as RootNode;

        return tree;
    }
}