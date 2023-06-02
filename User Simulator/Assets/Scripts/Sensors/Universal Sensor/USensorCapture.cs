using UnityEngine;
using UnityEngine.Perception.GroundTruth.DataModel;

namespace HIAAC.UserSimulator
{
    public abstract class USensorCapture : Sensor
    {
        USensorDefinition definition;
        public USensorCapture(USensorDefinition definition) 
            : base(definition)
        {
            this.definition = definition;
        }

        public USensorCapture(USensorDefinition definition, Transform transform)
            : base(definition, transform.position, transform.rotation)
        {
            this.definition = definition;
        }

        public USensorCapture(USensorDefinition definition, Transform transform,  Vector3 velocity, Vector3 acceleration)
            : base(definition, transform.position, transform.rotation, velocity, acceleration)
        {
            this.definition = definition;
        }
        
        public sealed override void ToMessage(IMessageBuilder builder)
        {
            base.ToMessage(builder);

            builder.AddString("groupID", definition.GroupID);

            AddToMessage(builder);
        }

        abstract protected void AddToMessage(IMessageBuilder builder);  
    }
}