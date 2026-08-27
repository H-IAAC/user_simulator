namespace HIAAC.UserSimulator
{
    public class RotationSensorDefinition : USensorDefinition
    {
        public override string modelType => "type.HIAAC.br/HIAAC.Position";

        /// <inheritdoc/>
        public RotationSensorDefinition(SensorInfo sensorInfo, PerceptionSensorProperties perceptionSensorProperties, string modality)
                : base(sensorInfo, perceptionSensorProperties, modality)
        {
        }

    }
}