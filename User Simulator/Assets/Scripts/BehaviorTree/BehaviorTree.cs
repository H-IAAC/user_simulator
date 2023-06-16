using UnityEngine;

[CreateAssetMenu(menuName="Behavior Tree/Behavior Tree")]
public class BehaviorTree: ScriptableObject
{
    public Node rootNode;
    public NodeState treeState = NodeState.Runnning;

    public NodeState Update()
    {
        if(rootNode.state == NodeState.Runnning)
        {
            treeState = rootNode.Update();
        }

        return treeState;
    }


}