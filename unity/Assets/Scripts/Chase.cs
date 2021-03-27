using UnityEngine;
using UnityEngine.AI;

public class Chase : MonoBehaviour
{
    public Transform target;

    void Start()
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        agent.destination = target.position;
    }
}
