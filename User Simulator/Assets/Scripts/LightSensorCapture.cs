using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Perception.GroundTruth.DataModel;

namespace HIAAC.UserSimulator
{
    public class LightSensorCapture : Sensor
    {
        public float lightIntensity;
        public LightSensorCapture(LightSensorDefinition definition, 
                            Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 acceleration,
                            float lightIntensity)
                : base(definition, position, rotation, velocity, acceleration)
        {
            this.lightIntensity = lightIntensity;
        }

        public override void ToMessage(IMessageBuilder builder)
        {
            base.ToMessage(builder);

            builder.AddFloat("lightIntensity", lightIntensity);
        }
    }
}
