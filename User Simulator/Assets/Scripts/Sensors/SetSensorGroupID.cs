using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HIAAC.UserSimulator
{
    /// <summary>
    /// Set the Group ID of the sensors in the game object or children.
    /// </summary>
    public class SetSensorGroupID : MonoBehaviour
    {
        [Tooltip("ID of the sensor group")]
        [SerializeField] string groupID = "";

        /// <summary>
        /// Sets the group ID of the sensors.
        /// </summary>
        void Awake()
        {
            groupID = GroupIDManager.addGroup(groupID);

            UniversalSensor[] sensors = GetComponentsInChildren<UniversalSensor>();

            foreach(UniversalSensor sensor in sensors)
            {
                sensor.GroupID = groupID;
            }
        }
    }
}
