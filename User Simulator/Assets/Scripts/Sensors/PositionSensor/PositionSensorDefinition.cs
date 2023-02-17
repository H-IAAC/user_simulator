namespace HIAAC.UserSimulator
{
    public class PositionSensorDefinition : USensorDefinition
    {
        public override string modelType => "type.HIAAC.br/HIAAC.Position";

        /// <inheritdoc/>
        public PositionSensorDefinition(SensorInfo sensorInfo, PerceptionSensorProperties perceptionSensorProperties, string modality)
                : base(sensorInfo, perceptionSensorProperties, modality)
        {
        }

    }
}