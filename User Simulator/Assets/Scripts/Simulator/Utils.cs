using UnityEngine;
using UnityEngine.Perception.GroundTruth;
using UnityEngine.Perception.GroundTruth.DataModel;
using Unity.MLAgents.Sensors;
using UnityEngine.Perception.Randomization.Scenarios;

public static class Utils
{
    public static bool PerceptionRunning
    {
        get
        {
            if(ScenarioBase.activeScenario == null)
            {
                return false;
            }

            return true;
        }
    }
}