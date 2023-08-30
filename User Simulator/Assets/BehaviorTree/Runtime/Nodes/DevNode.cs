using UnityEngine;
using System.Collections.Generic;

namespace HIAAC.BehaviorTree
{
    [System.Serializable]
    public class DataTest
    {
        public string name;
        public int value;
    }

    public class DevNode : ActionNode
    {
        [SerializeReference] public List<BlackboardProperty> dataTest = new();

        [SerializeField] FloatBlackboardProperty floatProperty = new();

        public DevNode()
        {
            CreateProperty(typeof(StringBlackboardProperty), "MyString");
            dataTest.Add(new FloatBlackboardProperty());

        }

        public override void OnStart()
        {

        }

        public override void OnStop()
        {
  
        }

        public override NodeState OnUpdate()
        {
            return NodeState.Success;
        }
    }
}