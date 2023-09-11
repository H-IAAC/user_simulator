using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceOnGPS : MonoBehaviour
{
    [SerializeField] MapReference map;
    [SerializeField] float lat;
    [SerializeField] float lon;
    [SerializeField] Vector3 rotation;

    bool placed = false;

    void LateUpdate()
    {
        if(!placed)
        {
            Vector3 position = MapboxUtils.gpsToUnity(map.map, lat, lon);

            transform.position = position;

            transform.rotation = Quaternion.Euler(rotation);

            placed = true;
            enabled = false;
        }
    }

    void OnValidate()
    {
        placed = false;
        enabled = true;
    }
}

//-22.8212061, -47.0667623
