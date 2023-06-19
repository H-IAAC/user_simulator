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

        return node;
    }
}