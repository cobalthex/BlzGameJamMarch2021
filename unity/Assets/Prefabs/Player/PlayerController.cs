using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float TurnSpeed = 300;
    public float MoveSpeed = 5;
    public float JumpStrength = 2;
    public bool InvertLookUp = false;

    private float yaw = 0;
    private float pitch = 0;
    private float roll = 0;

    private Vector3 jumpVector;

    private CharacterController characterController;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        bool groundedPlayer = characterController.isGrounded;
        if (groundedPlayer && jumpVector.y < 0)
        {
            jumpVector.y = 0f;
        }

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
        move += (transform.forward * Input.GetAxis("Vertical")
            + transform.right * Input.GetAxis("Horizontal"))
            * (MoveSpeed * Time.deltaTime);

        // jump
        if (Input.GetButtonDown("Jump") && groundedPlayer)
        {
            jumpVector.y += Mathf.Sqrt(JumpStrength * -1.0f * Physics.gravity.y);
        }

        // gravity
        jumpVector.y += Physics.gravity.y * Time.deltaTime;

        move += jumpVector * Time.deltaTime;
        characterController.Move(move);
    }
}
