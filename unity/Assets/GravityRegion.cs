using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityRegion : MonoBehaviour
{
    // todo: use local gravity
    public Vector3 InternalGravity = Physics.gravity;


    private void OnTriggerEnter(Collider other)
    {
        other.attachedRigidbody.useGravity = false;
    }

    private void OnTriggerExit(Collider other)
    {
        other.attachedRigidbody.useGravity = true;
    }

    private void OnTriggerStay(Collider other)
    {
        other.attachedRigidbody.AddForce(InternalGravity, ForceMode.Acceleration);
    }
}
