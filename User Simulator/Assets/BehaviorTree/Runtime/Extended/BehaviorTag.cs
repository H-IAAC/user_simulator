using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(menuName="Behavior Tree/Behavior Tag")]
public class BehaviorTag :  ScriptableObject
{
    public BehaviorTree tree;

    public List<BTagParameter> parameters;

    public List<BTagParameter> minimumValueParameters = new();
    public List<BTagParameter> maximumValueParameters = new();

    public bool IsCompatible(List<BTagParameter> parameters)
    {
        return BTagParameter.IsCompatible(parameters, minimumValueParameters, maximumValueParameters);
    }
}