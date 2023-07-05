using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Generic serializable event.
/// </summary>
[CreateAssetMenu(menuName ="HIAAC/Game Event")]
public class GameEvent : ScriptableObject
{

    readonly List<EventListener> eventListeners = 
        new List<EventListener>();

    [Tooltip("Sends a debug log when event is raised.")]
    [SerializeField] bool debugMode = false;
    

    /// <summary>
    /// Raises the event, calling the listeners.
    /// </summary>
    public void Raise()
    {
        if(debugMode)
        {
            Debug.Log("Game event "+this.name+" raised.");
        }

        for(int i = eventListeners.Count -1; i >= 0; i--)
        {
            eventListeners[i].OnEventRaised(this.name);
        }
    }

    /// <summary>
    /// Raises the event sending an argument to the listeners.
    /// </summary>
    /// <typeparam name="T">Type of the argument.</typeparam>
    /// <param name="arg">Argument</param>
    public void Raise<T>(T arg)
    {
        if(debugMode)
        {
            Debug.Log("Game event "+this.name+" raised with "+arg.GetType().Name+" object.");
        }

        for(int i = eventListeners.Count - 1; i >= 0; i--)
        {
            eventListeners[i].OnEventRaised<T>(this.name, arg);
        }
    }

    /// <summary>
    /// Registers a listener to be notified when the event is raised.
    /// </summary>
    /// <param name="listener">The listener to notify.</param>
    public void RegisterListener(EventListener listener)
    {
        if (!eventListeners.Contains(listener))
            eventListeners.Add(listener);
    }

    /// <summary>
    /// Unregisters a listener to be notified when the event is raised.
    /// </summary>
    /// <param name="listener">The listener to stop notifing.</param>

    public void UnregisterListener(EventListener listener)
    {
        if (eventListeners.Contains(listener))
            eventListeners.Remove(listener);
    }

    void OnValidate()
    {
        //Debug.Log(typeof(T));
    }
}