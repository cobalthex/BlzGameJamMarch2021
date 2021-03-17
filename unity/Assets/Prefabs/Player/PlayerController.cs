using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float TurnSpeed = 500;
    public float MoveSpeed = 10;
    public float JumpSeconds = 0.1f;
    public float JumpStrength = 30;
    public bool InvertLookUp = false;

    float jumpEndTime = 0;
    bool wasGrounded = true;

    float yaw = 0;
    float pitch = 0;
    float roll = 0;

    CharacterController characterController;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        // rotate camera
        {
            var turnSpeed = TurnSpeed * Time.deltaTime;

            yaw += Input.GetAxis("Mouse Y") * turnSpeed * (InvertLookUp ? 1 : -1);
            pitch += Input.GetAxis("Mouse X") * turnSpeed;

            yaw = Mathf.Clamp(yaw, -90, 90); // yaw

            transform.rotation = Quaternion.Euler(yaw, pitch, roll);
        }

        var move = new Vector3();

        // basic movement
        if (characterController.isGrounded)
        {
            move += (transform.forward * Input.GetAxis("Vertical")
                + transform.right * Input.GetAxis("Horizontal"))
                * (MoveSpeed * Time.deltaTime);

            if (Input.GetButtonDown("Jump"))
                jumpEndTime = Time.time + JumpSeconds;
        }
        wasGrounded = characterController.isGrounded;

        if (Time.time < jumpEndTime)
            move += transform.up * Mathf.Max(0, Input.GetAxis("Jump"))
                 * (JumpStrength * Time.deltaTime);

        // todo: handle velocity, character controller seems to be kinda stupid

        // gravity
        move += Physics.gravity * Time.deltaTime;

        characterController.Move(move);
    }

    // void OnGUI()
    // {
    // }
}
