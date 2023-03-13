using UnityEngine;
using UnityEngine.AI;
using HIAAC.ScriptableList;

[RequireComponent(typeof(IGoTo))]
public class TestAgent : MonoBehaviour
{
    [SerializeField] Vector3SList locations;
    
    IGoTo goToHandler;
    
    void Start()
    {
        goToHandler = GetComponent<IGoTo>();

        if(locations.Count > 0)
        {
            goToRandomTarget();
        }
        
    }

    void Update()
    {
        if(goToHandler.Ended)
        {
            goToRandomTarget();
        }
    }

    void goToRandomTarget()
    {
        Vector3 target = getRandomTarget();
        goToHandler.GoTo(target);
    }

    Vector3 getRandomTarget()
    {
        int index = Random.Range(0, locations.Count);
        return locations[index];
    }
}