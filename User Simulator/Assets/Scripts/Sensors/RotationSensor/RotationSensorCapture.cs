using UnityEngine;
using UnityEngine.Perception.GroundTruth.DataModel;

namespace HIAAC.UserSimulator
{
    public class RotationSensorCapture : USensorCapture
    {
        Vector3 eulerRotation;

        public RotationSensorCapture(USensorDefinition definition, Transform transform)
            : base(definition, transform)
        {
            eulerRotation = transform.rotation.eulerAngles;
        }

        protected override void AddToMessage(IMessageBuilder builder)
        {
            builder.AddFloatArray("eulerRotation", MessageBuilderUtils.ToFloatVector(eulerRotation));
        }
    }
}
