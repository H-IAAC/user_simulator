using System.Collections;
using System.Collections.Generic;
using UnityEngine.Perception.GroundTruth;

namespace HIAAC.UserSimulator
{
    public class GroupIDManager
    {
        List<string> groupIDs;
        Dictionary<string, int> idCount;

        static GroupIDManager instance;

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

        void Report()
        {
            DatasetCapture.ReportMetadata("groupIDs", groupIDs.ToArray());
        }
    }
    
}