using UnityEngine;
using UnityEngine.Perception.GroundTruth.DataModel;

namespace HIAAC.UserSimulator
{
    public class AccelSensorCapture : USensorCapture
    {
        Vector3 sensorFrameAcceleration;

        //definition, transform.position, transform.rotation, velocity, acceleration
        public AccelSensorCapture(USensorDefinition definition, Transform transform, Vector3 acceleration, Vector3 sensorFrameAcceleration) 
            : base(definition, transform, Vector3.zero, acceleration)
        {
            this.sensorFrameAcceleration = sensorFrameAcceleration;
        }

        protected override void AddToMessage(IMessageBuilder builder)
        {
            builder.AddFloatArray("sensorFrameAcceleration", MessageBuilderUtils.ToFloatVector(sensorFrameAcceleration));
        }
    }
}
