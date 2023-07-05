namespace HIAAC.UserSimulator
{
    /// <summary>
    /// Information about the sensor.
    /// </summary>
    [System.Serializable]
    public struct SensorInfo
    {   
        /// <summary>
        /// Sensor ID. Must be unique (enforced automatically).
        /// </summary>
        public string id;

        /// <summary>
        /// Description of the sensor.
        /// </summary>
        public string description;
        
        /// <summary>
        /// Group the sensor belongs.
        /// </summary>
        public string groupID;
    }
}