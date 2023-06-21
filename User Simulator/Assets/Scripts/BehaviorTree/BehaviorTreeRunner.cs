using UnityEngine;

public class BehaviorTreeRunner : MonoBehaviour
{
    [SerializeField] public BehaviorTree tree;

    void Start()
    {
        tree = tree.Clone();
        tree.Bind(gameObject);
    }

    void Update()
    {
        tree.Update();
    }

}