using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HIAAC.ScriptableList;

public class LocationToList : MonoBehaviour
{
    [SerializeField] Vector3SList list;

    void OnEnable()
    {
        list.Add(transform.position);
    }

    void OnDisable()
    {
        list.Remove(transform.position);
    }

}
