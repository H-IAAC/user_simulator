using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HIAAC.BehaviorTrees;

[RequireComponent(typeof(BehaviorTreeRunner))]
public class SwitchDaytime : MonoBehaviour
{
    BehaviorTreeRunner runner;

    void Start()
    {
        runner = GetComponent<BehaviorTreeRunner>();
    }

    public void Switch()
    {
        bool night = runner.GetBlackboardProperty<bool>("Night");
        night = !night;

        runner.SetBlackboardProperty("Night", night);

        if(night)
        {
            Debug.Log("Set time to night");
        }
        else
        {
            Debug.Log("Set time to day");
        }
        
    }
}
