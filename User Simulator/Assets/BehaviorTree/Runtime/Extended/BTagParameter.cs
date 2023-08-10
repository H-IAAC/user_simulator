using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BTagParameter
{
    public BTagParameterType type;
    [SerializeField] float value = 0;

    [SerializeField] public float Value
    {
        get
        {
            return value;
        }
        set
        {
            if(value < 0 || value > 1)
            {
                Debug.Log("Value must be in [0, 1].");
            }
            else
            {
                this.value = value;
            }
        }
    }

    public static bool IsCompatible(List<BTagParameter> parameters, List<BTagParameter> minimumValueParameters, List<BTagParameter> maximumValueParameters)
    {
        foreach(BTagParameter minConstraint in minimumValueParameters)
        {
            bool satisfied = false;
            foreach(BTagParameter parameter in parameters)
            {
                if(parameter.type == minConstraint.type && parameter.Value >= minConstraint.Value)
                {
                    satisfied = true;
                    break;
                }
            }

            if(!satisfied)
            {
                return false;
            }
        }
    
        foreach (BTagParameter parameter in parameters)
        {
            bool satisfied = true;
            foreach(BTagParameter maxConstraint in maximumValueParameters)
            {
                if(parameter.type == maxConstraint.type && parameter.Value > maxConstraint.Value)
                {
                    satisfied = false;
                    break;
                }
            }

            if(!satisfied)
            {
                return false;
            }
        }

        return true;
    }
}