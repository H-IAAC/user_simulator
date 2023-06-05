using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HIAAC.FromJSON;

public class LoadJSONTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void soCallback(ScriptableObject so)
    {
        print(so);

        PointTrajectory trajectory = (PointTrajectory) so;

        print("Start: "+trajectory.trajectory[0].ToString());
        print("End: "+trajectory.trajectory[trajectory.trajectory.Count-1].ToString());
    }

    public void askToUser()
    {
        FromJSON.askSO(soCallback);

    }
}
