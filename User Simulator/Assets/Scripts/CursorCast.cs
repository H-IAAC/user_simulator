using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class CursorCast : MonoBehaviour
{
    [Tooltip("Layers that can be hit by the cursor.")]
    [SerializeField] LayerMask layermask;

    [Tooltip("Max distance for hit check.")]
    [SerializeField] float maxDistance = 100;

    [Tooltip("Methods to invoke when a hit occur.")]
    [SerializeField] protected UnityEvent<RaycastHit, Vector2> onHit;
    
    [Tooltip("Show the caster array for debugging.")]
    [SerializeField] bool showDebugRay = false;

    Vector2 lastPosition;
    


    public void ReceiveCursorInput(InputAction.CallbackContext context)
    {
        Vector2 cursorPosition = context.ReadValue<Vector2>();        

        checkHit(cursorPosition);   

        lastPosition = cursorPosition;
    }

    void checkHit(Vector2 position)
    {
        Ray ray = Camera.main.ScreenPointToRay(position);
        RaycastHit hit;

        if(showDebugRay)
        {
            Debug.DrawRay(ray.origin, 100*ray.direction, Color.green);
        }
        

        if(Physics.Raycast(ray, out hit, maxDistance, layermask))
        {
            onHit.Invoke(hit, position);
        }

    }

    public bool checkIfHitting(RaycastHit oldHit)
    {
        Ray ray = Camera.main.ScreenPointToRay(lastPosition);
        
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, maxDistance, layermask))
        {
            if(GameObject.ReferenceEquals(hit.collider.gameObject, oldHit.collider.gameObject))
            {
                return true;
            }
        }

        return false;
    }
}