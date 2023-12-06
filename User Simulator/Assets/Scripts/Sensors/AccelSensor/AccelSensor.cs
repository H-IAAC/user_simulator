using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.Perception.GroundTruth.DataModel;

namespace HIAAC.UserSimulator
{
    /// <summary>
    /// Sensor for capture acceleration data.
    /// </summary>
    public class AccelSensor : UniversalSensor
    {
        Vector3 position, lastPosition;

        Vector3 velocity, lastVelocity;
        Vector3 acceleration;
        Vector3 sensorFrameAcceleration;
        
        Vector3 capturedAcceleration;
        Vector3 capturedSensorFrameAcceleration;

        float lastTime;

        public Vector3 Acceleration
        {
            get
            {
                return new Vector3
                {
                    x = acceleration.x,
                    y = acceleration.y,
                    z = acceleration.z
                };
            }
        }

        public Vector3 SensorFrameAcceleration
        {
            get
            {
                return new Vector3
                {
                    x = sensorFrameAcceleration.x,
                    y = sensorFrameAcceleration.y,
                    z = sensorFrameAcceleration.z
                };
            }
        }


        protected override void ResetSensorState()
        {
            position = transform.position;
            lastPosition = position;

            velocity = Vector3.zero;
            lastVelocity = Vector3.zero;
            
            acceleration = Vector3.zero;

            lastTime = Time.time;
        }      

        void FixedUpdate()
        {
            float time = Time.time;
            float deltaT = time - lastTime;

            lastPosition = position;
            lastVelocity = velocity;
            lastTime = time;

            position = new Vector3
            {
                x = transform.position.x,
                y = transform.position.y,
                z = transform.position.z
            };

            if(deltaT != 0) // Skip first frame
            {
                velocity = (position - lastPosition) / deltaT;
                acceleration = (velocity - lastVelocity) / deltaT; 
                sensorFrameAcceleration = transform.rotation* acceleration;
            }

            
        }

        protected override Sensor Capture(USensorDefinition sensorDefinition)
        {
            return new AccelSensorCapture(sensorDefinition, transform, capturedAcceleration, capturedSensorFrameAcceleration);
        }

        protected override void Compute()
        {
            capturedAcceleration = new Vector3
            {
                x = acceleration.x,
                y = acceleration.y,
                z = acceleration.z
            };

            capturedSensorFrameAcceleration = new Vector3
            {
                x = sensorFrameAcceleration.x,
                y = sensorFrameAcceleration.y,
                z = sensorFrameAcceleration.z
            };
        }


        protected override int Write(ObservationWriter writer)
        {
            writer.Add(capturedAcceleration);
            return 1;   
        }

        protected override ObservationSpec CreateObservationSpec()
        {
            return ObservationSpec.Vector(3);
        }

        protected override USensorDefinition CreateSensorDefinition(SensorInfo sensorInfo, PerceptionSensorProperties sensorProperties)
        {
            return new AccelSensorDefinition(sensorInfo, perceptionSensorProperties, "acceleration");
        }
    }
}

