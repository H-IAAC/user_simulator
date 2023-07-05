using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Trajectory of multiple points.
/// </summary>
[CreateAssetMenu(menuName ="HIAAC/Simulator/Point Trajectory")]
public class PointTrajectory : ScriptableObject
{
    [Tooltip("Points in the trajectory.")]
    [SerializeField] public List<Vector3> trajectory = new List<Vector3>();

    [Tooltip("Curve to interpolate between two points. Default is linear.")]
    [SerializeField ] public AnimationCurve interpolationCurve = AnimationCurve.Linear(0, 0, 1, 1);

    /// <summary>
    /// Samples the trajectory in some position.
    /// </summary>
    /// <param name="time">Time to sample the trajectory. Must be in [0, 1]. </param>
    /// <returns>Sampled trajectory point.</returns>
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

    /// <summary>
    /// Samples the trajectory in some position.
    /// </summary>
    /// <param name="index">Index of the starting point.</param>
    /// <param name="localTime">Time to sample the trajectory between the points. Must be in [0, 1].</param>
    /// <returns>Sampled trajectory point.</returns>
    public Vector3 Sample(int index, float localTime)
    {
        if(index >= trajectory.Count)
        {
            return trajectory[trajectory.Count-1];
        }

        Vector3 position = Vector3.Lerp(trajectory[index], trajectory[index+1], localTime);

        return position;
    }

    /// <summary>
    /// Converts the trajectory to a float array.
    /// </summary>
    /// <returns>Trajectory in float[][] representation.</returns>
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
