using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPanelCaller : MonoBehaviour
{
    [SerializeField] UITag uiTag;
    [SerializeField] GameEvent gameEvent;

    public UITag UITag
    {
        get
        {
            return uiTag;
        }
    }

    public void Show()
    {
        gameEvent.Raise<GameObject>(this.gameObject);
    }

    public void Hide()
    {
        gameEvent.Raise();
    }
}
