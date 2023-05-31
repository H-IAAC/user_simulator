using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Selectable : MonoBehaviour
{
    [Tooltip("Methods to invoke when starting selection.")]
    [SerializeField] UnityEvent onSelectStart;

    [Tooltip("Methods to invoke when ending selection.")]
    [SerializeField] UnityEvent onSelectEnd;

    public void StartSelect()
    {
        onSelectStart.Invoke();
    }

    public void EndSelect()
    {
        onSelectEnd.Invoke();
    }
}