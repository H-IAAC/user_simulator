namespace HIAAC.UserSimulator
{
    public class LightSensorDefinition : USensorDefinition
    {
        public override string modelType => "type.HIAAC.br/HIAAC.Light";

        /// <inheritdoc/>
        public LightSensorDefinition(SensorInfo sensorInfo, PerceptionSensorProperties perceptionSensorProperties, string modality)
                : base(sensorInfo, perceptionSensorProperties, modality)
        {
        }

    }
}