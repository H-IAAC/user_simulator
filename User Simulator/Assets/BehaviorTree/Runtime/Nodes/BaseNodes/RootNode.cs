using System.Collections.Generic;

public class RootNode : Node
{
    public Node child;

    public override void OnStart()
    {
        
    }

    public override void OnStop()
    {
        
    }

    public override NodeState OnUpdate()
    {
        return child.Update();
    }

    public override Node Clone()
    {
        RootNode node = Instantiate(this);
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
        this.child = null;
    }

    public override List<Node> GetChildren()
    {
        List<Node> children = new();
        
        if(child != null)
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