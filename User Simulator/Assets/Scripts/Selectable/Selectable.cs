using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Marks the object as selectable and invokes events when selected.
/// </summary>
public class Selectable : MonoBehaviour
{
    [Tooltip("Methods to invoke when starting selection.")]
    [SerializeField] UnityEvent onSelectStart;

    [Tooltip("Methods to invoke when ending selection.")]
    [SerializeField] UnityEvent onSelectEnd;

    /// <summary>
    /// Marks the object as beeing selected
    /// </summary>
    public void StartSelect()
    {
        onSelectStart.Invoke();
    }

    /// <summary>
    /// Marks the object as no longer selected
    /// </summary>
    public void EndSelect()
    {
        onSelectEnd.Invoke();
    }
}