using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Scriptable object that stores values.
/// </summary>
public class ScriptableVariable<T> : ScriptableObject
{
    T _value;

    public T value
    {
        get
        {
            return _value;
        }

        set
        {
            _value = value;
            ValueChange.Invoke();
        }
    }

    public UnityEvent ValueChange;
}