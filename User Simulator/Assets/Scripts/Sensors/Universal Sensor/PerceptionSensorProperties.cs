using UnityEngine.Perception.GroundTruth.DataModel;
using UnityEngine;

/// <summary>
/// Properties needed for a Perception sensor.
/// </summary>
[System.Serializable]
public struct PerceptionSensorProperties
{
    [Tooltip("Time between frames needed by the sensor. Cannot be 0.")]
    [SerializeProperty("SimulationDeltaTime")][SerializeField]
    float simulationDeltaTime;

    [Tooltip("Frames between sensor captures. 0 for capture every frame between SimulationDeltaTime")]
    [SerializeProperty("FramesBetweenCaptures")][SerializeField]
    int framesBetweenCaptures;

    [Tooltip("Frame in the sequence to start capturing the sensor")]
    [SerializeProperty("StartAtFrame")][SerializeField]
    int startAtFrame;

    [Tooltip("Mode for sensor captures.")]
    public CaptureTriggerMode captureTriggerMode;

    public PerceptionSensorProperties(float simulationDeltaTime, int framesBetweenCaptures, int startAtFrame, CaptureTriggerMode captureTriggerMode)
    {
        this.simulationDeltaTime = simulationDeltaTime;
        this.framesBetweenCaptures = framesBetweenCaptures;
        this.startAtFrame = startAtFrame;
        this.captureTriggerMode = captureTriggerMode;
    }

    /// <summary>
    /// Time between frames needed by the sensor. Cannot be 0.
    /// </summary>
    public float SimulationDeltaTime
    {
        get
        {
            return simulationDeltaTime;
        }
        set
        {
            if(value <= 0)
            {
                Debug.LogError("Sensor simulation delta time cannot be less or equal to zero.");
            }
            else
            {
                simulationDeltaTime = value;
            }
        }
    }

    /// <summary>
    /// Frames between sensor captures. 0 for capture every frame between SimulationDeltaTime
    /// </summary>
    public int FramesBetweenCaptures
    {
        get
        {
            return framesBetweenCaptures;
        }
        set
        {
            if(value < 0)
            {
                Debug.LogError("Sensor frames between captures cannot be less than zero.");
            }
            else
            {
                framesBetweenCaptures = value;
            }
        }
    }

    /// <summary>
    /// Frame in the sequence to start capturing the sensor
    /// </summary>
    public int StartAtFrame
    {
        get
        {
            return startAtFrame;
        }
        set
        {
            if(value < 0)
            {
                Debug.LogError("Sensor start at frame cannot be less than zero.");
            }
            else
            {
                startAtFrame = value;
            }
        }
    }
}