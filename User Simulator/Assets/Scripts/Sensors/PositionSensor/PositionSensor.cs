using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.Perception.GroundTruth.DataModel;

namespace HIAAC.UserSimulator
{
    public class PositionSensor : UniversalSensor
    {
        Vector3 position;

        protected override Sensor Capture(SensorDefinition sensorDefinition)
        {
            return new PositionSensorCapture(sensorDefinition, transform);
        }

        protected override void Compute()
        {
            position.x = transform.position.x;
            position.y = transform.position.y;
            position.z = transform.position.z;
        }

        protected override int Write(ObservationWriter writer)
        {
            writer.Add(position);
            return 3;   
        }

        protected override ObservationSpec CreateObservationSpec()
        {
            return ObservationSpec.Vector(3);
        }

        protected override SensorDefinition CreateSensorDefinition(SensorInfo sensorInfo, PerceptionSensorProperties sensorProperties)
        {
            return new PositionSensorDefinition(sensorInfo, perceptionSensorProperties, "position");
        }

        protected override void ResetSensorState()
        {
            
        }
    }
}

