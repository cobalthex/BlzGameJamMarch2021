using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerGravityFlip : MonoBehaviour
{
    public Vector3 GravityFront = Physics.gravity;
    public Vector3 GravityBehind = Physics.gravity;

    private void OnTriggerStay(Collider other)
    {
        var velocity = other.attachedRigidbody.velocity;
        var entranceDirection = Vector3.Dot(velocity, transform.forward);

        bool onFrontSide = entranceDirection >= 0;
        Physics.gravity = onFrontSide ? GravityFront : GravityBehind;
    }
}
