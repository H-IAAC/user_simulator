using UnityEngine;
using UnityEngine.Perception.GroundTruth.DataModel;

namespace HIAAC.UserSimulator
{
    public class LightSensorCapture : USensorCapture
    {
        public float lightIntensity;

        public LightSensorCapture(LightSensorDefinition definition, Transform transform, float lightIntensity)
            : base(definition, transform)
        {
            this.lightIntensity = lightIntensity;
        }

        protected override void AddToMessage(IMessageBuilder builder)
        {
            builder.AddFloat("lightIntensity", lightIntensity);
        }
    }
}
