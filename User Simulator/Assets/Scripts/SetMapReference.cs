using UnityEngine;

using Mapbox.Unity.Map;

public class SetMapReference : MonoBehaviour
{   
    [SerializeField] AbstractMap map;
    [SerializeField] MapReference mapReference;

    void Awake()
    {
        mapReference.map = map;
    }
}