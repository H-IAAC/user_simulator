using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// Listener to game event that call UnityEvents when raised.
/// </summary>
[AddComponentMenu("Shadow/Game Event Listener")]
public class GameEventListener : MonoBehaviour, EventListener
{
    [Tooltip("Event to register with.")]
    [SerializeField] protected GameEvent Event;

    [Tooltip("Time to wait before raising response, in seconds.")]
    [SerializeField] protected float delay = 0;

    [Tooltip("Response to invoke when Event is raised.")]
    [SerializeField] UnityEvent Response;

    

    /// <summary>
    /// Register listener to the event.
    /// </summary>
    private void OnEnable()
    {
        Event.RegisterListener(this);
    }

    /// <summary>
    /// Unregister listener to the event.
    /// </summary>
    private void OnDisable()
    {
        Event.UnregisterListener(this);
    }

    /// <summary>
    /// Call the local UnityEvents.
    /// </summary>
    /// <param name="eventName">Name of the raised GameEvent.</param>
    public void OnEventRaised(string eventName)
    {
        if(gameObject.activeInHierarchy)
        {
            StartCoroutine("RaiseResponse");
        }
        
    }

    /// <summary>
    /// Call the local UnityEvents.
    /// </summary>
    /// <typeparam name="T">Type of the argument</typeparam>
    /// <param name="eventName">Name of the event</param>
    /// <param name="arg">Argument</param>
    public void OnEventRaised<T>(string eventName, T arg)
    {
        if(gameObject.activeInHierarchy)
        {
            IEnumerator coroutine = RaiseResponse<T>(arg);
            StartCoroutine(coroutine);
        }
        
    }

    /// <summary>
    /// Waits the delay and call the event listeners.
    /// </summary>
    protected virtual IEnumerator RaiseResponse()
    {
        if(delay > 0)
        {
            yield return new WaitForSeconds(delay);
        }
        
        Response.Invoke();
    }

    /// <summary>
    /// Waits the delay and call the event listeners.
    /// </summary>
    /// <typeparam name="T">Type of the argument</typeparam>
    /// <param name="arg">Argument</param>
    protected virtual IEnumerator RaiseResponse<T>(T arg)
    {
        if(delay > 0)
        {
            yield return new WaitForSeconds(delay);
        }        
        Response.Invoke();
    }
}