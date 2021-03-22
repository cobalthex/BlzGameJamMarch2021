using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float TurnSpeed = 500;
    public float MoveSpeed = 5f;
    public float JumpSeconds = 0.1f;
    public float JumpStrength = 30;
    public bool InvertLookUp = false;

    float yaw;
    float pitch;
    float roll;

    float jumpEndTime = 0;

    private Vector3 jumpVector;

    new Rigidbody rigidbody;
    new Camera camera;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        camera = GetComponentInChildren<Camera>();
    }

    bool isGrounded;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
            return;
        }

        // rotate camera
        {
            var turnSpeed = TurnSpeed * Time.deltaTime;

            yaw += Input.GetAxis("Mouse Y") * turnSpeed * (InvertLookUp ? 1 : -1);
            pitch += Input.GetAxis("Mouse X") * turnSpeed;

            var newTilt = 0;
            if (Input.GetKey(KeyCode.Q))
                newTilt = -1;
            if (Input.GetKey(KeyCode.E))
                newTilt = 1;
            if (newTilt != 0)
            {
                roll = Mathf.Clamp(roll + (newTilt * Time.deltaTime * TurnSpeed), -30, 30);
                // todo: broken
                camera.transform.Rotate(Vector3.forward, roll, Space.Self);
            }

            yaw = Mathf.Clamp(yaw, -90, 90); // yaw

            camera.transform.localRotation = Quaternion.Euler(yaw, 0, roll);
            transform.rotation = Quaternion.AngleAxis(pitch, transform.up);
        }

        var move = new Vector3();

        // not very reliable
        // should probably place collider at feet and test if colliding
        Physics.Raycast(new Ray(transform.position, Physics.gravity.normalized), out var hit);
        isGrounded = hit.distance <= 0.001f;

        // basic movement
        move += (Vector3.forward * Input.GetAxis("Vertical"));
        move += (Vector3.right * Input.GetAxis("Horizontal"));
        move.Normalize();
        move *= MoveSpeed * (isGrounded ? 1 : 0.1f);

        if (isGrounded)
        {
            if (Input.GetButtonDown("Jump"))
                jumpEndTime = Time.time + JumpSeconds;
        }

#if DEBUG
        if (Input.GetKeyDown(KeyCode.G))
            jumpEndTime = Time.time + JumpSeconds;
#endif

        if (Time.time < jumpEndTime)
        {
            move += transform.up /* * Input.GetAxis("Jump") */ * JumpStrength;
        }

        // todo: handle velocity, character controller seems to be kinda stupid

        // use MovePosition?
        rigidbody.AddRelativeForce(move);

    }

    //void OnCollisionEnter(Collision collision)
    //{
    //    isGrounded = Mathf.Abs(Vector3.Dot(rigidbody.velocity, Physics.gravity)) < 0.001f;
    //}

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 100, 30), $"Grounded: {isGrounded}");
    }
}
