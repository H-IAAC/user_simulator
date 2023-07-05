using System.Collections.Generic;
using UnityEngine;

public abstract class CompositeNode : Node
{
    [HideInInspector] 
    public List<Node> children = new();

    public CompositeNode(MemoryMode memoryMode = MemoryMode.Memoryless) : base(memoryMode)
    {

    }

    public override Node Clone()
    {
        CompositeNode node = Instantiate(this);

        node.children = new List<Node>();

        foreach(Node child in children)
        {
            node.children.Add(child.Clone());
        }

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
    }

    public override List<Node> GetChildren()
    {
        return children;
    }
}