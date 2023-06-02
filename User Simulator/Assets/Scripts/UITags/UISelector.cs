using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct UIPanelInfo
{
    public UITag tag;
    public UIInfoPanel panel;
}

public class UISelector : MonoBehaviour
{
    GameObject currentPanel;

    [SerializeField] List<UIPanelInfo> panels;

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
