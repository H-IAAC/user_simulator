using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data of a available to use info panel.
/// </summary>
[System.Serializable]
public struct UIPanelInfo
{
    [Tooltip("UITag type that the panel handle.")]
    public UITag tag;

    [Tooltip("Panel to show the object information.")]
    public UIInfoPanel panel;
}

/// <summary>
/// Manages the available info panels
/// </summary>
public class UIPanelManager : MonoBehaviour
{
    GameObject currentPanel;

    [Tooltip("Available information panels to show objects information.")]
    [SerializeField] List<UIPanelInfo> panels;

    /// <summary>
    /// Shows the panel for a object.
    /// </summary>
    /// <param name="targetObject">Object to show information. If null, hides the current panel.</param>
    public void ShowPanel(GameObject targetObject)
    {
        if(targetObject == null)
        {
            if(currentPanel != null)
            {
                currentPanel.SetActive(false);
            }

            return;
        }

        UITag targetTag = targetObject.GetComponent<UIPanelCaller>().UITag;

        foreach(UIPanelInfo info in panels)
        {
            if(info.tag == targetTag)
            {
                info.panel.gameObject.SetActive(true);
                info.panel.Show(targetObject);

                currentPanel = info.panel.gameObject;

                break;
            }
        }

    }
}
