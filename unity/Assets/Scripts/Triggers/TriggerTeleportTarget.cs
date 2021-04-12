using UnityEngine;
using UnityEngine.AI;

public class TriggerTeleportTarget : MonoBehaviour
{
    public string FilterTag = "Player";

    public GameObject Teleportee;
    public Transform Destination;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(FilterTag))
            return;

        var navAgent = Teleportee.GetComponent<NavMeshAgent>();
        if (navAgent != null)
            navAgent.enabled = false;

        Teleportee.transform.SetPositionAndRotation(Destination.position, Destination.rotation);

        if (navAgent != null)
        {
            navAgent.ResetPath();
            navAgent.enabled = true;
        }
    }
}
