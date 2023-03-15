using UnityEngine;
using UnityEngine.AI;
using HIAAC.ScriptableList;

public enum LocationSelectionMode
{
    SEQUENTIAL,
    RANDOM
}

[RequireComponent(typeof(IGoTo))]
public class TestAgent : MonoBehaviour
{
    [SerializeField] Vector3SList locations;
    [SerializeField] LocationSelectionMode locationSelectionMode = LocationSelectionMode.RANDOM;
    
    IGoTo goToHandler;

    int index = 0;
    
    void Start()
    {
        goToHandler = GetComponent<IGoTo>();

        if(locations.Count > 0)
        {
            selectTarget();
        }
        
    }

    void Update()
    {
        if(goToHandler.Ended)
        {
            selectTarget();
        }
    }

    void selectTarget()
    {
        Vector3 target;
        switch(locationSelectionMode)
        {
            case LocationSelectionMode.SEQUENTIAL:
                target = selectSequentialTarget();
            break;

            case LocationSelectionMode.RANDOM:
                target = selectRandomTarget();
            break;

            default:
                target = Vector3.zero;
            break;
        }

        goToHandler.GoTo(target);
    }

    Vector3 selectSequentialTarget()
    {
        index += 1;
        
        if(index >= locations.Count)
        {
            index = 0;
        }

        return locations[index];
    }

    Vector3 selectRandomTarget()
    {
        index = Random.Range(0, locations.Count);

        return locations[index];
    }
}