using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[AddComponentMenu("Shadow/Game Event Listener")]
public class GameEventListener : MonoBehaviour, EventListener
{
    [Tooltip("Event to register with.")]
    [SerializeField] protected GameEvent Event;

    [Tooltip("Time to wait before raising response, in seconds.")]
    [SerializeField] protected float delay = 0;

    [Tooltip("Response to invoke when Event is raised.")]
    [SerializeField] UnityEvent Response;

    


    private void OnEnable()
    {
        Event.RegisterListener(this);
    }

    private void OnDisable()
    {
        Event.UnregisterListener(this);
    }

    public void OnEventRaised(string eventName)
    {
        if(gameObject.activeInHierarchy)
        {
            StartCoroutine("RaiseResponse");
        }
        
    }

    public void OnEventRaised<T>(string eventName, T arg)
    {
        if(gameObject.activeInHierarchy)
        {
            IEnumerator coroutine = RaiseResponse<T>(arg);
            StartCoroutine(coroutine);
        }
        
    }

    protected virtual IEnumerator RaiseResponse()
    {
        if(delay > 0)
        {
            yield return new WaitForSeconds(delay);
        }
        
        Response.Invoke();
    }

    protected virtual IEnumerator RaiseResponse<T>(T arg)
    {
        if(delay > 0)
        {
            yield return new WaitForSeconds(delay);
        }        
        Response.Invoke();
    }
}