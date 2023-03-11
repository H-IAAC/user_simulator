using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NavMeshGoTo : MonoBehaviour, IGoTo
{
    NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void GoTo(Vector3 destination)
    {
        agent.SetDestination(destination);
    }

    public bool Ended
    {
        get
        {
            return agent.remainingDistance <= agent.stoppingDistance;
        }
    }
}