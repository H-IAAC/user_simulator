using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace HIAAC.BehaviorTree
{
    [CreateAssetMenu(menuName = "Behavior Tree/Behavior Tag")]
    public class BehaviorTag : ScriptableObject
    {
        public BehaviorTree tree;

        public TagLifecycleType onSuccess = TagLifecycleType.HOLD;
        public TagLifecycleType onFailure = TagLifecycleType.DROP;
        public TagLifecycleType onRunning = TagLifecycleType.HOLD;

        public List<BTagParameter> parameters;

        public List<BTagParameter> minimumValueParameters = new();
        public List<BTagParameter> maximumValueParameters = new();

        public bool IsCompatible(List<BTagParameter> parameters)
        {
            return BTagParameter.IsCompatible(parameters, minimumValueParameters, maximumValueParameters);
        }

        public void OnValidate()
        {
            if (onRunning == TagLifecycleType.OVERRIDABLE)
            {
                Debug.LogWarning("Cannot override on running. Changing to HOLD");
                onRunning = TagLifecycleType.HOLD;
            }
        }
    }
}