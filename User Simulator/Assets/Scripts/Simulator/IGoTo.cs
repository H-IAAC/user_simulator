using UnityEngine;

public interface IGoTo
{
    public sealed void GoTo(Vector3 destination)
    {
        if(enabled)
        {
            goToImplementation(destination);
        }
    }

    protected void goToImplementation(Vector3 destination);

    public bool Ended{get;}

    public Vector3 Destination{get;}

    public bool enabled{get; set;}

}