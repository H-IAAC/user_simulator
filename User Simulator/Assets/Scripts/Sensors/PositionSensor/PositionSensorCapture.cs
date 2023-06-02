using UnityEngine;
using UnityEngine.Perception.GroundTruth.DataModel;

namespace HIAAC.UserSimulator
{
    public class PositionSensorCapture : USensorCapture
    {
        public PositionSensorCapture(USensorDefinition definition, Transform transform) 
            : base(definition, transform)
        {
        }

        protected override void AddToMessage(IMessageBuilder builder)
        {
            
        }
    }
}
