using UnityEngine;
using UnityEngine.Perception.GroundTruth.DataModel;

namespace HIAAC.UserSimulator
{
    public abstract class USensorCapture : Sensor
    {
        public USensorCapture(SensorDefinition definition) 
            : base(definition)
        {

        }

        public USensorCapture(SensorDefinition definition, Transform transform)
            : base(definition, transform.position, transform.rotation)
        {

        }

        public USensorCapture(SensorDefinition definition, Transform transform,  Vector3 velocity, Vector3 acceleration)
            : base(definition, transform.position, transform.rotation, velocity, acceleration)
        {

        }
        
        public sealed override void ToMessage(IMessageBuilder builder)
        {
            base.ToMessage(builder);

            AddToMessage(builder);
        }

        abstract protected void AddToMessage(IMessageBuilder builder);  
    }
}