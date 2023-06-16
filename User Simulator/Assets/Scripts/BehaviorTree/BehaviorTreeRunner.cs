using UnityEngine;

public class BehaviorTreeRunner : MonoBehaviour
{
    BehaviorTree tree;

    void Start()
    {
        tree = ScriptableObject.CreateInstance<BehaviorTree>();

        DebugLogNode logNode = ScriptableObject.CreateInstance<DebugLogNode>();
        logNode.message = "TEST DEBUG NODE";

        tree.rootNode = logNode;
    }

    void Update()
    {
        tree.Update();
    }

}