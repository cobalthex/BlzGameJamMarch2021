using UnityEngine;
using UnityEngine.AI;

public class Chase : MonoBehaviour
{
    public Transform target;
    public float updateSeconds;

    private float updateCountdown;
    private NavMeshAgent agent;
    private bool isPartial;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.destination = target.position;
        updateCountdown = updateSeconds;
        isPartial = false;
    }

    void FixedUpdate()
    {
        if (agent.pathStatus == NavMeshPathStatus.PathPartial && !isPartial)
        {
            Debug.Log("Can't reach dest anymore");
            isPartial = true;
        }
        else if (agent.pathStatus == NavMeshPathStatus.PathComplete && isPartial)
        {
            Debug.Log("Can now reach dest");
            isPartial = false;
        }
        

        updateCountdown -= Time.fixedDeltaTime;
        if (updateCountdown <= 0)
        {
            agent.destination = target.position;
            updateCountdown = updateSeconds;
        }
    }
}
