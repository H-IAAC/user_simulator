using UnityEngine;
using UnityEngine.AI;
using HIAAC.ScriptableList;

[RequireComponent(typeof(NavMeshAgent))]
public class TestAgent : MonoBehaviour
{
    [SerializeField] Vector3SList locations;
    
    NavMeshAgent agent;
    
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if(locations.Count > 0)
        {
            goToRandomTarget();
        }
        
    }

    void Update()
    {
        if(agent.remainingDistance <= agent.stoppingDistance)
        {
            goToRandomTarget();
        }
    }

    void goToRandomTarget()
    {
        Vector3 target = getRandomTarget();
        agent.SetDestination(target);
    }

    Vector3 getRandomTarget()
    {
        int index = Random.Range(0, locations.Count);
        return locations[index];
    }
}