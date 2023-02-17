using UnityEngine.Perception.GroundTruth.DataModel;
using UnityEngine;

[System.Serializable]
public struct PerceptionSensorProperties
{
    [SerializeProperty("SimulationDeltaTime")][SerializeField]
    float simulationDeltaTime;

    [SerializeProperty("FramesBetweenCaptures")][SerializeField]
    int framesBetweenCaptures;

    [SerializeProperty("StartAtFrame")][SerializeField]
    int startAtFrame;

    public CaptureTriggerMode captureTriggerMode;

    public PerceptionSensorProperties(float simulationDeltaTime, int framesBetweenCaptures, int startAtFrame, CaptureTriggerMode captureTriggerMode)
    {
        this.simulationDeltaTime = simulationDeltaTime;
        this.framesBetweenCaptures = framesBetweenCaptures;
        this.startAtFrame = startAtFrame;
        this.captureTriggerMode = captureTriggerMode;
    }

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