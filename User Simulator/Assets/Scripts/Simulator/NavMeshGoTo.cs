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

    void IGoTo.goToImplementation(Vector3 destination)
    {
        if(agent == null)
        {
            agent = GetComponent<NavMeshAgent>();

            if(agent == null)
            {
                return;
            }
        }
        
        agent.SetDestination(destination);
    }

    public bool Ended
    {
        get
        {
            if(agent == null)
            {
                return true;
            }

            return agent.remainingDistance <= agent.stoppingDistance;
        }
    }

    public Vector3 Destination
    {
        get
        {
            return agent.destination;
        }
    }

    void OnDisable()
    {
        agent.isStopped = true;   
    }

}