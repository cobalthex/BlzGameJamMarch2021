using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunChaser : MonoBehaviour
{
    public float StunTime = 1f;

    private void OnCollisionEnter(Collision collision)
    {
        if (enabled &&
            collision.collider.TryGetComponent<Chase>(out var chaser))
        {
            chaser.Stun();
            Destroy(gameObject);
        }
        enabled = false;
    }
}
