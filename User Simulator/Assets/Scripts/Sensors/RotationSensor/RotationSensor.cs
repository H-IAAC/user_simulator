using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.Perception.GroundTruth.DataModel;

namespace HIAAC.UserSimulator
{
    /// <summary>
    /// Sensor for capture rotation data.
    /// </summary>
    public class RotationSensor : UniversalSensor
    {
        Vector3 eulerRotation;
        Quaternion rotation;

        protected override Sensor Capture(USensorDefinition sensorDefinition)
        {
            return new RotationSensorCapture(sensorDefinition, transform);
        }

        protected override void Compute()
        {
            rotation = new Quaternion(transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w);
            eulerRotation = transform.rotation.eulerAngles;
        }

        protected override int Write(ObservationWriter writer)
        {
            writer.Add(rotation);
            writer.Add(eulerRotation);
            return 1;
        }

        protected override ObservationSpec CreateObservationSpec()
        {
            return ObservationSpec.Vector(7);
        }

        protected override USensorDefinition CreateSensorDefinition(SensorInfo sensorInfo, PerceptionSensorProperties sensorProperties)
        {
            return new RotationSensorDefinition(sensorInfo, perceptionSensorProperties, "rotation");
        }

        protected override void ResetSensorState()
        {

        }
    }
}

