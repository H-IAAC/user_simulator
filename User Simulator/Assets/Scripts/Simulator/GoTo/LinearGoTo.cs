using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Goes to the destination moving linearly towards it
/// (timed linear interpolation between the current and target position).
/// </summary>
public class LinearGoTo : MonoBehaviour, IGoTo
{   
    [SerializeProperty("Velocity")][Tooltip("The movement velocity in Unity_units/s")][SerializeField] 
    float velocity = 3.5f;

    Vector3 origin;
    Vector3 destination;

    float startTime = 0;
    float duration = 0;
    float endTime = 0;

    NavMeshAgent agent;
    public bool Ended
    {
        get
        {
            return Time.time > endTime;
        }
    }

    public Vector3 Destination
    {
        get
        {
            return destination;
        }
    }

    /// <summary>
    /// Velocity of the movement.
    /// </summary>
    public float Velocity
    {
        set
        {
            velocity = value;
            origin = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            recomputeTime();
        }
        get
        {
            return velocity;
        }
    }

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void OnEnable()
    {
        checkNavMeshAgent();
    }

    void Update()
    {
        //Checks if ended
        if(Time.time > endTime)
        {
            return;
        }

        float t = (Time.time-startTime)/duration;

        if(t>1 || t<0)
        {
            Debug.LogError("WRONG t");
        }

        //Updates the current position
        Vector3 currentPosition = Vector3.Lerp(origin, destination, t);

        this.transform.position = currentPosition;
    }

    /// <summary>
    /// Updates the desired destination.
    /// </summary>
    /// <param name="destination">Desired destination</param>
    void IGoTo.goToImplementation(Vector3 destination)
    {
        origin = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        this.destination = new Vector3(destination.x, destination.y, destination.z);

        recomputeTime();
    }


    /// <summary>
    /// Recomputes the interpolation times.
    /// </summary>
    void recomputeTime()
    {
        if(origin == null || destination == null)
        {
            startTime = 0;
            duration = 0;
            endTime = 0;
            return;
        }

        startTime = Time.time;
        duration = Vector3.Distance(origin, destination)/velocity;
        endTime = startTime+duration;
    }

    /// <summary>
    /// Checks if the object have an enabled NavMeshAgent
    /// </summary>
    void checkNavMeshAgent()
    {
        if(agent != null)
        {
            if(agent.enabled == true)
            {
                Debug.LogWarning("LinearGoTo doesn't work with NavMeshAgent. Disabling agent.");
                agent.enabled = false;
            }
        }
    }
}