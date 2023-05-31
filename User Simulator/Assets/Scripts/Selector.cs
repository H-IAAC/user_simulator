using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class Selector : CursorCast
{   
    [SerializeField] bool onlyDeselectWhenOver = true;
    bool selecting;
    Selectable selectedObject;
    RaycastHit lastHit;

    RaycastHit selectedHit;

    void Start()
    {
        selecting = false;

        this.onHit.AddListener(ReceiveCursorHit);
    }
    
    public void HandleSelectAction(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started)
        {
            if(!selecting)
            {
                EnableSelect();
            }
            else
            {
                DisableSelect();
            }
        }
    }


    public void EnableSelect()
    {
        if(lastHit.transform != null)
        {
            if(!checkIfHitting(lastHit))
            {
                return;
            }

            Selectable selectable = null;

            Selectable hitSelectable = lastHit.transform.gameObject.GetComponent<Selectable>();
            if(hitSelectable != null)
            {
                selectable = hitSelectable;
            }
            else
            {
                Selectable colliderSelectable = lastHit.collider.gameObject.GetComponent<Selectable>();
                selectable = colliderSelectable;
            }

            if(selectable == null)
            {
                return;
            }

            selecting = true;
            selectedObject = selectable;
            selectedHit = lastHit;

            selectedObject.StartSelect();
        }

    }

    public void DisableSelect()
    {
        if(selectedObject != null)
        {
            if(onlyDeselectWhenOver && !checkIfHitting(selectedHit))
            {
                return;
            }

            selectedObject.EndSelect();
            selecting = false;
        }       
    }

    public void ReceiveCursorHit(RaycastHit hit, Vector2 position)
    {
        lastHit = hit;
    }
}