using UnityEngine;
using UnityEngine.Perception.GroundTruth.DataModel;

namespace HIAAC.UserSimulator
{
    /// <summary>
    /// Base class for a capture of an UniversalSensor using the Perception library.
    /// </summary>
    public abstract class USensorCapture : Sensor
    {
        USensorDefinition definition;

        /// <summary>
        /// USensorCapture constructor.
        /// </summary>
        /// <param name="definition">Sensor definition</param>
        public USensorCapture(USensorDefinition definition) 
            : base(definition)
        {
            this.definition = definition;
        }

        /// <summary>
        /// USensorCapture constructor.
        /// </summary>
        /// <param name="definition">Sensor definition</param>
        /// <param name="transform">Sensor transform with position and rotation.</param>
        public USensorCapture(USensorDefinition definition, Transform transform)
            : base(definition, transform.position, transform.rotation)
        {
            this.definition = definition;
        }

        /// <summary>
        /// USensorCapture constructor.
        /// </summary>
        /// <param name="definition">Sensor definition</param>
        /// <param name="transform">Sensor transform with position and rotation.</param>
        /// <param name="velocity">Sensor velocity</param>
        /// <param name="acceleration">Sensor acceleration</param>
        public USensorCapture(USensorDefinition definition, Transform transform,  Vector3 velocity, Vector3 acceleration)
            : base(definition, transform.position, transform.rotation, velocity, acceleration)
        {
            this.definition = definition;
        }

        /// <summary>
        /// Writes the capture to the message builder.
        /// </summary>
        /// <param name="builder">Builder to write the capture data</param>        
        public sealed override void ToMessage(IMessageBuilder builder)
        {
            base.ToMessage(builder);

            builder.AddString("groupID", definition.GroupID);

            AddToMessage(builder);
        }

        /// <summary>
        /// Adds specific sensor data to the builder.
        /// </summary>
        /// <param name="builder">Builder to write the capture data</param>
        abstract protected void AddToMessage(IMessageBuilder builder);  
    }
}