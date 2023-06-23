using System.Collections.Generic;
using UnityEngine;

public abstract class ActionNode : Node
{
    public ActionNode(MemoryMode memoryMode = MemoryMode.Memoryless) : base(memoryMode)
    {

    }

    public override void AddChild(Node child)
    {

    }

    public override void RemoveChild(Node child)
    {

    }

    public override List<Node> GetChildren()
    {
        return new List<Node>();
    }
}