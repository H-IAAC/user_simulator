using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Scriptable object that stores values.
/// </summary>
public class ScriptableVariable<T> : ScriptableObject
{
    T _value;

    /// <summary>
    /// Stored value.
    /// </summary>
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

    /// <summary>
    /// Event raised when the stored value changes.
    /// </summary>
    public UnityEvent ValueChange;
}