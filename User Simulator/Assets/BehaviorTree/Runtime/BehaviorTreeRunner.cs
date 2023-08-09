using System;
using UnityEngine;

public class BehaviorTreeRunner : MonoBehaviour
{
    [SerializeField] public BehaviorTree tree;

    void Start()
    {
        Debug.Log(tree.nodes.Count);
        tree = tree.Clone();
        Debug.Log(tree.nodes.Count);
        tree.Bind(gameObject);
        Debug.Log(tree.nodes.Count);
    }

    void Update()
    {
        tree.Update();
    }

    public void SetBlackboardProperty(string name, object value)
    {
        int index = tree.blackboard.FindIndex(x => x.PropertyName == name);
        if(index < 0)
        {
            throw new ArgumentException("Property does not exist in tree.");
        }

        tree.blackboard[index].Value = value;
    }

    public T GetBlackboardProperty<T>(string name)
    {
        int index = tree.blackboard.FindIndex(x => x.PropertyName == name);
        if(index < 0)
        {
            throw new ArgumentException("Property does not exist in tree.");
        }

        return (T)tree.blackboard[index].Value;
    }
}