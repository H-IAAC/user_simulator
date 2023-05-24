using UnityEngine;
using UnityEngine.Perception.GroundTruth;
using UnityEngine.Perception.GroundTruth.DataModel;
using Unity.MLAgents.Sensors;
using UnityEngine.Perception.Randomization.Scenarios;

namespace HIAAC.UserSimulator
{
    public abstract class UniversalSensor : SensorComponent, ISensor
    {
        [SerializeField] protected SensorInfo sensorInfo;
        
        [SerializeField] protected PerceptionSensorProperties perceptionSensorProperties = 
                            new PerceptionSensorProperties(0.0166f, 0, 0, CaptureTriggerMode.Scheduled);
        
        protected abstract void Compute();
        protected abstract void ResetSensorState();

        protected abstract Sensor Capture(USensorDefinition sensorDefinition);
        protected abstract int Write(ObservationWriter writer);
        protected abstract USensorDefinition CreateSensorDefinition(SensorInfo sensorInfo, PerceptionSensorProperties sensorProperties);

        protected abstract ObservationSpec CreateObservationSpec();


        void Awake()
        {
            observationSpec = CreateObservationSpec();
        }

        void Start()
        {
            DatasetCapture.SimulationEnding += ResetSensorState;
        }

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
        

        //------------------------------------------------------------------------------------------------------------
        //Perception things
        SensorHandle sensorHandle;
        USensorDefinition sensorDefinition;
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

        void ISensor.Reset()
        {
            ResetSensorState();
        }

        void ISensor.Update()
        {
            Compute();
        }

        ObservationSpec ISensor.GetObservationSpec()
        {
            return observationSpec;
        }

        string ISensor.GetName()
        {
            return sensorInfo.id;
        }

        CompressionSpec ISensor.GetCompressionSpec()
        {
            return CompressionSpec.Default();
        }

        byte[] ISensor.GetCompressedObservation()
        {
            throw new System.NotImplementedException();
        }

        int ISensor.Write(ObservationWriter writer)
        {
            return Write(writer);
        }

        //Sensor Component
        public override ISensor[] CreateSensors()
        {
            ISensor[] thisList = {this};
            return thisList;
        }

        
    }

}