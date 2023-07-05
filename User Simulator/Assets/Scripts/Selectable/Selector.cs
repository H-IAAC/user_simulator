using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Enable selecting objects in the screen
/// </summary>
public class Selector : CursorCast
{   
    [Tooltip("If true, only delects a selected object when cursor is over it.")]
    [SerializeField] bool onlyDeselectWhenOver = true;
    
    [Tooltip("If true, allows selecting a new object without deselecting the previous object.")]
    [SerializeField] bool allowToggle = true;
    bool selecting;
    Selectable selectedObject;
    RaycastHit lastHit;

    RaycastHit selectedHit;

    void Start()
    {
        selecting = false;

        this.onHit.AddListener(ReceiveCursorHit);
    }
    
    /// <summary>
    /// Handles the select action.
    /// </summary>
    /// <param name="context">Context of the select action</param>
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

    /// <summary>
    /// Checks if is hitting any object, and marks it as selected.
    /// </summary>
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

    /// <summary>
    /// Deselect the object if is selecting anything.
    /// </summary>
    public void DisableSelect()
    {
        if(selectedObject != null)
        {
            if(allowToggle)
            {
                Selectable lastHitSelectable = getSelectable(lastHit);
                if(lastHitSelectable != selectedObject && checkIfHitting(lastHit))
                {
                    selectedObject.EndSelect();
                    selectedObject = lastHitSelectable;
                    selectedHit = lastHit;
                    selectedObject.StartSelect();
                    return;
                }
            }
            
            if(onlyDeselectWhenOver && !checkIfHitting(selectedHit))
            {
                return;
            }

            selectedObject.EndSelect();
        }

        selecting = false;       
    }

    /// <summary>
    /// Receives the cursor hit information.
    /// </summary>
    /// <param name="hit">Hit information.</param>
    /// <param name="position">Position of the cursor when casting</param>
    public void ReceiveCursorHit(RaycastHit hit, Vector2 position)
    {
        lastHit = hit;
    }

    /// <summary>
    /// Checks if the hit has any Selectable and returns it.
    /// </summary>
    /// <param name="hit">Hit to check</param>
    /// <returns>S</returns>
    Selectable getSelectable(RaycastHit hit)
    {
        Selectable selectable = null;

        //Check RigidBody object
        Selectable hitSelectable = lastHit.transform.gameObject.GetComponent<Selectable>();
        if(hitSelectable != null)
        {
            selectable = hitSelectable;
        }
        else //Check collider object
        {
            Selectable colliderSelectable = lastHit.collider.gameObject.GetComponent<Selectable>();
            selectable = colliderSelectable;
        }

        return selectable;
    }
}