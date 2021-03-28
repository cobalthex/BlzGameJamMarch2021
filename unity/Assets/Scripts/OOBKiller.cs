using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OOBKiller : MonoBehaviour
{
    void OnTriggerExit(Collider collider)
    {
        // check distance first?

        Debug.Log($"{collider} fell out of bounds");
        Destroy(collider.gameObject);
    }
}
