using UnityEngine;

public class BehaviorTreeRunner : MonoBehaviour
{
    [SerializeField] BehaviorTree tree;

    void Start()
    {
        tree = tree.Clone();
    }

    void Update()
    {
        tree.Update();
    }

}