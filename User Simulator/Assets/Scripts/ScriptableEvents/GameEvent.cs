using System.Collections.Generic;
using UnityEngine;
using System;


[CreateAssetMenu(menuName ="HIAAC/Game Event")]
public class GameEvent : ScriptableObject
{

    readonly List<EventListener> eventListeners = 
        new List<EventListener>();

    [Tooltip("Sends a debug log when event is raised.")]
    [SerializeField] bool debugMode = false;

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

    public void RegisterListener(EventListener listener)
    {
        if (!eventListeners.Contains(listener))
            eventListeners.Add(listener);
    }

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