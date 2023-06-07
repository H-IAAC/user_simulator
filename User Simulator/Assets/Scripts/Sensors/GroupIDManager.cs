using System.Collections;
using System.Collections.Generic;
using UnityEngine.Perception.GroundTruth;

namespace HIAAC.UserSimulator
{
    /// <summary>
    /// Singleton to manage group IDs.
    /// </summary>
    public class GroupIDManager
    {
        List<string> groupIDs;
        Dictionary<string, int> idCount;

        static GroupIDManager instance;

        /// <summary>
        /// Adds an group ID, creating a unique ID.
        /// </summary>
        /// <param name="groupID">The base group ID.</param>
        /// <returns>Unique ID if the groupID is already in use.</returns>
        public static string addGroup(string groupID)
        {
            if(instance == null)
            {
                instance = new GroupIDManager();
            }

            if(instance.idCount.ContainsKey(groupID))
            {
                int n = instance.idCount[groupID];
                instance.idCount[groupID] += 1;
                groupID += n.ToString();
            }
            else
            {
                instance.idCount[groupID] = 1;
            }
                
            instance.groupIDs.Add(groupID);

            return groupID;
        }

        private GroupIDManager()
        {
            DatasetCapture.SimulationEnding += Report;

            groupIDs = new List<string>();
            idCount = new Dictionary<string, int>();
        }

        /// <summary>
        /// Adds the groups IDs to generate dataset metadata
        /// </summary>
        void Report()
        {
            DatasetCapture.ReportMetadata("groupIDs", groupIDs.ToArray());
        }
    }
    
}