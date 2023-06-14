using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Goes to the desired position using NavMesh
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class NavMeshGoTo : MonoBehaviour, IGoTo
{
    NavMeshAgent agent;

    void Awake()
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

            if(agent.enabled == false)
            {
                agent.enabled = true;
            }
        }
        
        agent.SetDestination(destination);
    }

    public bool Ended
    {
        get
        {
            if(agent == null || agent.enabled == false)
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

    void OnEnable()
    {
        if(agent.enabled == false)
        {
            agent.enabled = true;
        }
    }

    void OnDisable()
    {
        if(agent.enabled == true)
        {
            agent.isStopped = true;
        }
    }

}