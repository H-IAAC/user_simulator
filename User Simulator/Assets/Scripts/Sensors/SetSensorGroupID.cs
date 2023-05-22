using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HIAAC.UserSimulator
{
    public class SetSensorGroupID : MonoBehaviour
    {
        [SerializeField] string groupID = "";

        void Awake()
        {
            UniversalSensor[] sensors = GetComponentsInChildren<UniversalSensor>();

            foreach(UniversalSensor sensor in sensors)
            {
                sensor.SensorID = groupID+"_"+sensor.SensorID;
            }
        }
    }
}
