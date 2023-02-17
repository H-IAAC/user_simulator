using UnityEngine;
using UnityEngine.Perception.GroundTruth;
using UnityEngine.Perception.GroundTruth.DataModel;
using Unity.MLAgents.Sensors;

namespace HIAAC.UserSimulator
{
    

    public class LightSensor : UniversalSensor
    {
        float lightIntensity;

        void OnEnable()
        {
            Debug.LogWarning("Light sensor is not full implemented. Ligh intensity is always 255.");
        }

        protected override Sensor Capture(SensorDefinition sensorDefinition)
        {
            LightSensorDefinition LsensorDefinition = (LightSensorDefinition) sensorDefinition;
            return new LightSensorCapture(LsensorDefinition, transform, lightIntensity);
        }

        protected override SensorDefinition CreateSensorDefinition(SensorInfo sensorInfo, PerceptionSensorProperties sensorProperties)
        {
            return new LightSensorDefinition(sensorInfo, sensorProperties, "lightSensor");
        }

        protected override ObservationSpec CreateObservationSpec()
        {
            return ObservationSpec.Vector(1);
        }

        protected override void Compute()
        {
            lightIntensity = 255;
        }

        protected override void ResetSensorState()
        {
           
        }

        protected override int Write(ObservationWriter writer)
        {
            float[] obs = {lightIntensity}; 
            writer.AddList(obs);
            return 1;
        }
    }
}