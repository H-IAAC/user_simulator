using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="HIAAC/Simulator/Point Trajectory")]
public class PointTrajectory : ScriptableObject
{
    [SerializeField] public List<Vector3> trajectory = new List<Vector3>();
    [SerializeField ] public AnimationCurve interpolationCurve = AnimationCurve.Linear(0, 0, 1, 1);

    public Vector3 Sample(float time)
    {
        int count = trajectory.Count - 1;
        int index = (int) time*count;

        if(index + 1 >= trajectory.Count)
        {
            return trajectory[count];
        }

        float start = ((float)index)/count; 
        float end = ((float)(index+1))/count;
        float localTime = (float) ((time-start)/(end-start));

        return Sample(index, localTime);
    }

    public Vector3 Sample(int index, float localTime)
    {
        if(index >= trajectory.Count)
        {
            return trajectory[trajectory.Count-1];
        }

        Vector3 position = Vector3.Lerp(trajectory[index], trajectory[index+1], localTime);

        return position;
    }

    [SerializeField] public float[][] AsFloatArray
    {
        set
        {
            if(value[0].GetLength(0) != 3)
            {
                throw new ArgumentException("Float array dimension 1 lenght should be 3.");
            }


            this.trajectory.Capacity = value.GetLength(0);

            for(int i = 0; i< value.GetLength(0); i++)
            {
                trajectory.Add(new Vector3(value[i][0], value[i][1], value[i][2]));
            }
        }
    } 
}
