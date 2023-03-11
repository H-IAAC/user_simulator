using UnityEngine;

public interface IGoTo
{
    public void GoTo(Vector3 destination);

    public bool Ended{get;}
}