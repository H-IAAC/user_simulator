using UnityEngine;
using UnityEngine.Perception.GroundTruth.DataModel;

namespace HIAAC.UserSimulator
{
    public class USensorDefinition : SensorDefinition
    {
        public override string modelType => "type.HIAAC.br/HIAAC.USensor";
        
        public USensorDefinition(SensorInfo sensorInfo, PerceptionSensorProperties perceptionSensorProperties, string modality)
            : base(sensorInfo.id, perceptionSensorProperties.captureTriggerMode, sensorInfo.description, 
                    perceptionSensorProperties.StartAtFrame, perceptionSensorProperties.FramesBetweenCaptures, 
                    false, modality, perceptionSensorProperties.SimulationDeltaTime)
        {
            if(modelType == "type.HIAAC.br/HIAAC.USensor")
            {
                Debug.LogError("modelType sensor definition should be override to correct sensor model type");
            }
        }

        public override void ToMessage(IMessageBuilder builder)
        {
            base.ToMessage(builder);
            
            builder.AddInt("capture_size", 1);
        }
    }
}