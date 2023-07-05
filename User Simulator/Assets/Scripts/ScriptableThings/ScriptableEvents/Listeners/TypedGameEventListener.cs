using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Listener to game event that call UnityEvents when raised and supports argument casting.
/// </summary>
public class TypedGameEventListener<T> : GameEventListener
{
    [Tooltip("Response to invoke when Event is raised.")]
    [SerializeField]  UnityEvent<T> TypedResponse;

    [SerializeField] UnityEventBase baseEvent;

    public TypedGameEventListener()
    {
        baseEvent = new UnityEvent<float>();
    }

    /// <summary>
    /// Raises the local UnityEvents using the default type value.
    /// </summary>
    override protected IEnumerator RaiseResponse()
    {
        if(delay > 0)
        {
            yield return new WaitForSeconds(delay);
        }

        Debug.LogWarning("Typed GameEventListener being called without argument. Invoking response with default argument.");
        TypedResponse.Invoke(default);
    }

    /// <summary>
    /// Raises the local UnityEvents, casting the arguments before calling.
    /// </summary>
    /// <typeparam name="T2">Type of the argument used for raising the event.</typeparam>
    /// <param name="arg">Argument of the event.</param>
    /// <returns></returns>
    override protected IEnumerator RaiseResponse<T2>(T2 arg)
    {
        if(delay > 0)
        {
            yield return new WaitForSeconds(delay);
        }
        
        if(arg is T)
        {
            T castedArg = (T) (object) arg;
            TypedResponse.Invoke(castedArg);
        }
        else
        {
            string warningText = "Typed GameEventListener being called with wrong object type.\n";
            warningText += "Cannot cast from "+typeof(T2).FullName+" to "+typeof(T).FullName+".";
            warningText += " Invoking response with default argument.";
            Debug.LogWarning(warningText);

            TypedResponse.Invoke(default);
        }

        
    }
}
