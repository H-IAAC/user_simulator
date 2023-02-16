using UnityEngine;
using UnityEngine.Perception.GroundTruth;

namespace HIAAC.UserSimulator
{
    public class LightSensor : MonoBehaviour
    {
        [SerializeField] string id;
        [SerializeField] string description;

        SensorHandle sensorHandle;
        LightSensorDefinition sensorDefinition;

        void OnEnable()
        {
            Debug.LogWarning("Light sensor is not full implemented. Ligh intensity is always 255.");
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
                LightSensorCapture capture = new LightSensorCapture(sensorDefinition, transform.position, transform.rotation, Vector3.zero, Vector3.zero, 255);
                sensorHandle.ReportSensor(capture);  
            }
        }


        void EnsureSensorRegistered()
        {
            if(sensorHandle.IsNil)
            {
                sensorDefinition = new LightSensorDefinition(id, "sensor", description);
                sensorDefinition.simulationDeltaTime = 2.0f*0.0166f;
                sensorHandle = DatasetCapture.RegisterSensor(sensorDefinition);
            }
        }
    }
}