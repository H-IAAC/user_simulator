using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class TypedGameEventListener<T> : GameEventListener
{
    [Tooltip("Response to invoke when Event is raised.")]
    [SerializeField]  UnityEvent<T> TypedResponse;

    [SerializeField] UnityEventBase baseEvent;

    public TypedGameEventListener()
    {
        baseEvent = new UnityEvent<float>();
    }

    override protected IEnumerator RaiseResponse()
    {
        if(delay > 0)
        {
            yield return new WaitForSeconds(delay);
        }

        Debug.LogWarning("Typed GameEventListener being called without argument. Invoking response with default argument.");
        TypedResponse.Invoke(default);
    }

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
