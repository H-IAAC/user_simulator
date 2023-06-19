using System.Collections.Generic;
using UnityEngine;

public abstract class CompositeNode : Node
{
    [HideInInspector] public List<Node> children = new List<Node>();

    public override Node Clone()
    {
        CompositeNode node = Instantiate(this);
        
        foreach(Node child in children)
        {
            node.children.Add(child.Clone());
        }

        return node;
    }
}