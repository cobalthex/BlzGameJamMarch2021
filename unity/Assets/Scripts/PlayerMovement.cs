using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    void Start()
    {
    }

    void Update()
    {
        var forwardSpeed = Input.GetAxis("Vertical");
        var lateralSpeed = Input.GetAxis("Horizontal");
        
        
    }
}
