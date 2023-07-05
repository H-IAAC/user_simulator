using UnityEngine;

/// <summary>
/// Calls a UIPanel to show the object info.
/// 
/// Add to objects that needs to show information.
/// </summary>
public class UIPanelCaller : MonoBehaviour
{
    [Tooltip("UITag with type of the information panel to use.")]
    [SerializeField] UITag uiTag;

    [Tooltip("Game event to call when needs to show the object info.")]
    [SerializeField] GameEvent gameEvent;

    /// <summary>
    /// UITag with type of the information panel to use.
    /// </summary>
    public UITag UITag
    {
        get
        {
            return uiTag;
        }
    }

    /// <summary>
    /// Shows the object info panel.
    /// </summary>
    public void Show()
    {
        gameEvent.Raise<GameObject>(this.gameObject);
    }

    /// <summary>
    /// Hides the object info panel (hides any panel if other object is beeing show).
    /// </summary>
    public void Hide()
    {
        gameEvent.Raise();
    }
}
