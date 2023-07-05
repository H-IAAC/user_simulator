using UnityEngine;
using UnityEngine.Perception.GroundTruth;
using UnityEngine.Perception.GroundTruth.DataModel;
using Unity.MLAgents.Sensors;
using Unity.MLAgents;
using UnityEngine.Perception.Randomization.Scenarios;

public static class Utils
{
    /// <summary>
    /// Checks if the Perception library is running.
    /// </summary>
    public static bool PerceptionRunning
    {
        get
        {
            return !(ScenarioBase.activeScenario == null);
        }
    }

    /// <summary>
    /// Checks if the MLAgents library is running.
    /// </summary>
    public static bool MLAgentsRunning
    {
        get
        {
            return Academy.IsInitialized;
        }
    }
}