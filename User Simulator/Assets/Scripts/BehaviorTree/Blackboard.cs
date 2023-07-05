using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Blackboard
{
    public Vector3 moveToPosition;
    public Dictionary<string, object> keyValueMap = new();
}