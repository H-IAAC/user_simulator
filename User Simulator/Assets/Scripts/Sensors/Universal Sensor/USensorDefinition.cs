using UnityEngine;
using UnityEngine.Perception.GroundTruth.DataModel;

namespace HIAAC.UserSimulator
{
    /// <summary>
    /// Base class for a sensor definition of an UniversalSensor
    /// </summary>
    public abstract class USensorDefinition : SensorDefinition
    {
        /// <summary>
        /// Model type of the sensor
        /// </summary>
        public override string modelType => "type.HIAAC.br/HIAAC.USensor";

        string groupID = "";
        
        /// <summary>
        /// USensorDefinition constructor
        /// </summary>
        /// <param name="sensorInfo">Information of the sensor</param>
        /// <param name="perceptionSensorProperties">Sensor capture parameters for the Perception library.</param>
        /// <param name="modality">Modality of the sensor</param>
        public USensorDefinition(SensorInfo sensorInfo, PerceptionSensorProperties perceptionSensorProperties, string modality)
            : base(sensorInfo.id, perceptionSensorProperties.captureTriggerMode, sensorInfo.description, 
                    perceptionSensorProperties.StartAtFrame, perceptionSensorProperties.FramesBetweenCaptures, 
                    false, modality, perceptionSensorProperties.SimulationDeltaTime)
        {
            if(modelType == "type.HIAAC.br/HIAAC.USensor")
            {
                Debug.LogError("modelType sensor definition should be override to correct sensor model type");
            }

            this.groupID = sensorInfo.groupID;
        }

        /// <summary>
        /// Writes the sensor defintion to the builder.
        /// </summary>
        /// <param name="builder">Builder to write the definition.</param>
        public override void ToMessage(IMessageBuilder builder)
        {
            base.ToMessage(builder);
            
            builder.AddInt("capture_size", 1);
            builder.AddString("groupID", groupID);

        }

        /// <summary>
        /// ID of the sensor group.
        /// </summary>
        public string GroupID
        {
            get
            {
                return groupID;
            }
        }
    }
}