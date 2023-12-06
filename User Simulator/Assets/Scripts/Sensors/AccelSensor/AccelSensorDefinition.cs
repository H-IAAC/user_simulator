namespace HIAAC.UserSimulator
{
    public class AccelSensorDefinition : USensorDefinition
    {
        public override string modelType => "type.HIAAC.br/HIAAC.Acceleration";

        /// <inheritdoc/>
        public AccelSensorDefinition(SensorInfo sensorInfo, PerceptionSensorProperties perceptionSensorProperties, string modality)
                : base(sensorInfo, perceptionSensorProperties, modality)
        {
        }

    }
}