using UnityEngine;
using UnityEngine.Perception.GroundTruth;
using UnityEngine.Perception.GroundTruth.DataModel;
using Unity.MLAgents.Sensors;
using UnityEngine.Perception.Randomization.Scenarios;

namespace HIAAC.UserSimulator
{
    /// <summary>
    /// Sensor abstraction for working with the Perception and MLAgents packages.
    /// </summary>
    public abstract class UniversalSensor : SensorComponent, ISensor
    {
        [Tooltip("Description of the sensor.")]
        [SerializeField] protected SensorInfo sensorInfo;
        
        /// <summary>
        /// Properties for Perception captures mode.
        /// </summary>
        [SerializeField] protected PerceptionSensorProperties perceptionSensorProperties = 
                            new PerceptionSensorProperties(0.0166f, 0, 0, CaptureTriggerMode.Scheduled);
        
        /// <summary>
        /// Must update the sensor readings.
        /// </summary>
        protected abstract void Compute();

        /// <summary>
        /// Resets the sensor state.
        /// </summary>
        protected abstract void ResetSensorState();

        /// <summary>
        /// Creates a new capture (<see cref="HIAAC.UserSimulator.USensorCapture">USensorCapture</see>) with the sensor data.
        /// </summary>
        /// <param name="sensorDefinition">Definition created by the sensor.</param>
        /// <returns></returns>
        protected abstract Sensor Capture(USensorDefinition sensorDefinition);

        /// <summary>
        /// Writes the sensor data with the ObservationWriter.
        /// </summary>
        /// <param name="writer">Writer to send sensor data.</param>
        /// <returns>Size of the observation.</returns>
        protected abstract int Write(ObservationWriter writer);

        /// <summary>
        /// Creates the sensor definition.
        /// </summary>
        /// <param name="sensorInfo">Sensor info with basic sensor data.</param>
        /// <param name="sensorProperties">Properties of the sensor.</param>
        /// <returns>Definition of the sensor.</returns>
        protected abstract USensorDefinition CreateSensorDefinition(SensorInfo sensorInfo, PerceptionSensorProperties sensorProperties);

        /// <summary>
        /// Creates the ObservationSpec with observation type and size.
        /// </summary>
        /// <returns>Created ObservationSpec</returns>
        protected abstract ObservationSpec CreateObservationSpec();


        /// <summary>
        /// Creates the observation spec in object initialization.
        /// </summary>
        void Awake()
        {
            observationSpec = CreateObservationSpec();
        }

        /// <summary>
        /// Adds the reset sensor to simulation ending event.
        /// </summary>
        void Start()
        {
            DatasetCapture.SimulationEnding += ResetSensorState;
        }

        /// <summary>
        /// Capture the sensor data if Perception is running.
        /// </summary>
        void LateUpdate()
        {
            EnsureSensorRegistered();
            if (!sensorHandle.IsValid)
            {
                return;
            }
            
            if(sensorHandle.ShouldCaptureThisFrame)
            {
                Compute();
                Sensor capture = Capture(sensorDefinition);
                sensorHandle.ReportSensor(capture);  
            }
        }

        /// <summary>
        /// ID of the group the sensor belongs.
        /// </summary>
        public string GroupID
        {
            set
            {
                if(!sensorHandle.IsNil)
                {
                    Debug.LogWarning("Setting sensor group ID after simulation started will not be reflected in the dataset");
                }
                
                this.sensorInfo.groupID = value;
            }
            get
            {
                return this.sensorInfo.groupID;
            }
        }

        /// <summary>
        /// ID of the sensor.
        /// </summary>
        public string ID
        {
            get
            {
                return sensorInfo.id;
            }
        }
        

        //------------------------------------------------------------------------------------------------------------
        //Perception things
        SensorHandle sensorHandle;
        USensorDefinition sensorDefinition;

        /// <summary>
        /// Request the sensor to register a new capture. 
        /// Can only be used when the sensor is in the Manual capture trigger mode.
        /// </summary>
        public void RequestCapture()
        {
            if (perceptionSensorProperties.captureTriggerMode.Equals(CaptureTriggerMode.Manual))
            {
                sensorHandle.RequestCapture();
            }
            else
            {
                Debug.LogError($"{nameof(RequestCapture)} can only be used if the sensor is in " +
                    $"{nameof(CaptureTriggerMode.Manual)} capture mode.");
            }
        }

        /// <summary>
        /// Ensure the sensor is registred when there is an active Perception scenario.
        /// </summary>
        void EnsureSensorRegistered()
        { 
            if(ScenarioBase.activeScenario == null)
            {
                return;
            }
            if(sensorHandle.IsNil)
            {
                sensorDefinition = CreateSensorDefinition(sensorInfo, perceptionSensorProperties);
                sensorHandle = DatasetCapture.RegisterSensor(sensorDefinition);
            }
        }
      
        //---------------------------------------------------------------------------------------------------------------------------
        // ML Agents things

        //ISensor
        ObservationSpec observationSpec;

        /// <summary>
        /// Reset the sensor state
        /// </summary>
        void ISensor.Reset()
        {
            ResetSensorState();
        }

        /// <summary>
        /// Update the sensor reading value
        /// </summary>
        void ISensor.Update()
        {
            Compute();
        }

        /// <summary>
        /// Get the MLAgents observation specification.
        /// </summary>
        /// <returns>MLAgents observation specification</returns>
        ObservationSpec ISensor.GetObservationSpec()
        {
            return observationSpec;
        }


        /// <summary>
        /// Get the sensor name (same as ID).
        /// </summary>
        /// <returns>Sensor name/ID</returns>
        string ISensor.GetName()
        {
            return sensorInfo.id;
        }

        /// <summary>
        /// Get the sensor MLAgents compression specification.
        /// </summary>
        /// <returns>MLAgents compression specification</returns>
        CompressionSpec ISensor.GetCompressionSpec()
        {
            return CompressionSpec.Default();
        }

        byte[] ISensor.GetCompressedObservation()
        {
            throw new System.NotImplementedException();
        }


        /// <summary>
        /// Writes the sensor data with the ObservationWriter.
        /// </summary>
        /// <param name="writer">Writer to send sensor data.</param>
        /// <returns>Size of the observation.</returns>
        int ISensor.Write(ObservationWriter writer)
        {
            return Write(writer);
        }

        //Sensor Component

        /// <summary>
        /// Create the ISensor objects of the sensor.
        /// </summary>
        /// <returns>ISensor objects of the sensor.</returns>
        public override ISensor[] CreateSensors()
        {
            ISensor[] thisList = {this};
            return thisList;
        }

        
    }

}