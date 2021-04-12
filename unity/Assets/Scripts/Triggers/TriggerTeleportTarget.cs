using UnityEngine;

public class TriggerTeleportTarget : MonoBehaviour
{
    public string FilterTag = "Player";

    public GameObject Teleportee;
    public Transform Destination;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(FilterTag))
            return;

        Teleportee.transform.SetPositionAndRotation(Destination.position, Destination.rotation);
    }
}
