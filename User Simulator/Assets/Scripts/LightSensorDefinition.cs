using UnityEngine.Perception.GroundTruth.DataModel;

namespace HIAAC.UserSimulator
{
    public class LightSensorDefinition : SensorDefinition
    {
        public override string modelType => "type.HIAAC.br/HIAAC.Light";

        /// <inheritdoc/>
        public LightSensorDefinition(string id, string modality, string description)
                : base(id, modality, description)
        {
        }

        /// <inheritdoc/>
        public LightSensorDefinition(string id, CaptureTriggerMode captureTriggerMode, string description, float firstCaptureFrame, int framesBetweenCaptures, bool manualSensorsAffectTiming, string modality, float simulationDeltaTime, bool useAccumulation = false)
            : base(id, captureTriggerMode, description, firstCaptureFrame, framesBetweenCaptures, manualSensorsAffectTiming, modality, simulationDeltaTime)
        {

        }
    }
}