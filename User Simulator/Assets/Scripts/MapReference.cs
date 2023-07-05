using UnityEngine;

using Mapbox.Unity.Map;

[CreateAssetMenu(menuName ="HIAAC/MapReference")]
public class MapReference : ScriptableObject
{   
    public AbstractMap map{get; set;}
}