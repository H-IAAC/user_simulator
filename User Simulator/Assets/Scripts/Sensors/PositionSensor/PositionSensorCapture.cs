using UnityEngine;
using UnityEngine.Perception.GroundTruth.DataModel;

namespace HIAAC.UserSimulator
{
    public class PositionSensorCapture : USensorCapture
    {
        public PositionSensorCapture(SensorDefinition definition, Transform transform) 
            : base(definition, transform)
        {
        }

        protected override void AddToMessage(IMessageBuilder builder)
        {
            
        }
    }
}
