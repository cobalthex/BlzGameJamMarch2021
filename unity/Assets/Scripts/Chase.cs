using UnityEngine;
using UnityEngine.AI;

public class Chase : MonoBehaviour
{
    public Transform target;
    public float updateSeconds;

    private float updateCountdown;
    private NavMeshAgent agent;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.destination = target.position;
        updateCountdown = updateSeconds;
    }

    void FixedUpdate()
    {
        updateCountdown -= Time.fixedDeltaTime;
        if (updateCountdown <= 0)
        {
            agent.destination = target.position;
            updateCountdown = updateSeconds;
        }
    }
}
