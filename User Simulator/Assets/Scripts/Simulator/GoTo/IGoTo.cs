using UnityEngine;

/// <summary>
/// Interface to go to stategies for moving an object to some position.
/// </summary>
public interface IGoTo
{
    /// <summary>
    /// Goes to a destination.
    /// </summary>
    /// <param name="destination">Desired destination to go</param>
    public sealed void GoTo(Vector3 destination)
    {
        if(enabled)
        {
            goToImplementation(destination);
        }
    }

    /// <summary>
    /// Actual GoTo stategy implementation
    /// </summary>
    /// <param name="destination">Desired destination to go</param>
    protected void goToImplementation(Vector3 destination);

    /// <summary>
    /// True if arrived at the destination (or can't )
    /// </summary>
    public bool Ended{get;}

    /// <summary>
    /// Current destination.
    /// </summary>
    public Vector3 Destination{get;}

    /// <summary>
    /// Current stategy state.
    /// </summary>
    public bool enabled{get; set;}

}