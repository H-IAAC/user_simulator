using UnityEngine;

/// <summary>
/// Base class for creating info panels
/// </summary>
public abstract class UIInfoPanel : MonoBehaviour
{
    public abstract void Show(GameObject target);
}