using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private float floorEpsilon = 0.03f;

    public float LookSpeed = 30;
    public float MoveSpeed = 1;
    public float MaxMovementSpeed = 8;
    public float JumpSeconds = 0.1f;
    public float JumpStrength = 1;
    public bool InvertLookUp = false;

    float jumpEndTime = 0;

    Transform feet;
    Rigidbody physicsBody;

    Transform model;
    new Camera camera;

    Picker picker;

    public Hand LeftHand;
    public Hand RightHand;

    // vector3? local gravity

    void Start()
    {
        feet = transform.Find("feet");
        physicsBody = GetComponent<Rigidbody>();
        model = transform.Find("body");
        camera = GetComponentInChildren<Camera>();
        picker = GetComponent<Picker>();

        camera.nearClipPlane = 0.0001f; // editor only allows to 0.01

        // ensure the active camera
        StartCoroutine(WaitAndSetPlayerCamera());
    }

    bool isGrounded;
    float crouchValue = 0;

    IEnumerator WaitAndSetPlayerCamera()
    {
        yield return null;
        camera.enabled = false;
        camera.enabled = true;
    }

    void Update()
    {
#if DEBUG
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
            return;
        }
#endif

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }

        Look();

        #region Movement

        var move = new Vector3();

        // not very reliable
        // should probably place collider at feet and test if colliding
        var down = Physics.gravity.normalized;
        Physics.Raycast(new Ray(feet.position - (down * floorEpsilon), down), out var hit);
        isGrounded = hit.distance <= floorEpsilon;

        // basic movement
        move += (Vector3.forward * Input.GetAxis("Vertical"));
        move += (Vector3.right * Input.GetAxis("Horizontal"));
        move.Normalize();

        var forwardSpeed = Vector3.Dot(transform.rotation * move, physicsBody.velocity);
        var moveSpeed = Mathf.Lerp(MoveSpeed, 0, forwardSpeed / MaxMovementSpeed);

        move *= moveSpeed * (isGrounded ? 1 : 0.4f);

        if (isGrounded)
        {
            if (Input.GetButtonDown("Jump"))
                jumpEndTime = Time.time + JumpSeconds;
        }
        // fake ground friction? (vs lower air resistance)?

#if DEBUG
        if (Input.GetKeyDown(KeyCode.G))
            jumpEndTime = Time.time + JumpSeconds;
#endif

        if (Time.time < jumpEndTime)
        {
            move += transform.up /* * Input.GetAxis("Jump") */ * JumpStrength;
        }

        physicsBody.AddRelativeForce(move, ForceMode.Impulse);

        #endregion

        bool isCrouching = Input.GetButton("Crouch");
        crouchValue = Mathf.Lerp(crouchValue, isCrouching ? 1 : 0, 0.25f);

        transform.localScale = new Vector3(1, 1 - crouchValue * 0.5f, 1);
        LeftHand.transform.localScale = RightHand.transform.localScale = new Vector3(1, 1 / transform.localScale.y, 1);

        #region Interaction

        if (Input.GetButtonDown("Fire1"))
            Interact(LeftHand);
        else if (Input.GetKeyDown(KeyCode.Z))
            LeftHand.EquippedItem = null;

        if (Input.GetButtonDown("Fire2"))
            Interact(RightHand);
        else if (Input.GetKeyDown(KeyCode.C))
            RightHand.EquippedItem = null;

        #endregion
    }

    private void Look()
    {
        if (Cursor.lockState == CursorLockMode.None)
            return;

        var lookSpeed = LookSpeed * Time.unscaledDeltaTime;

        var q = camera.transform.localRotation; // transform.localEulerAngles are absoluted so calc manually

        //var yaw = Mathf.Asin(-2.0f * (q.x * q.z - q.w * q.y)) * Mathf.Rad2Deg;
        var pitch = Mathf.Atan2(2.0f * (q.y * q.z + q.w * q.x), q.w * q.w - q.x * q.x - q.y * q.y + q.z * q.z) * Mathf.Rad2Deg;
        var roll = Mathf.Atan2(2.0f * (q.x * q.y + q.w * q.z), q.w * q.w + q.x * q.x - q.y * q.y - q.z * q.z) * Mathf.Rad2Deg;

        // horizontal rotation
        {
            transform.Rotate(Vector3.up, Input.GetAxis("Mouse X") * lookSpeed, Space.World);
        }

        // vertical rotation
        {
            /* the hard way */
            // // quaternion rotations are always unsigned
            // var oldPitch = Quaternion.Angle(camera.transform.localRotation, Quaternion.AngleAxis(0, Vector3.right));
            // // there is probably a smarter way to do this
            // var dot = Quaternion.Dot(camera.transform.localRotation, Quaternion.AngleAxis(90, Vector3.right));
            // dot = (dot * 2) - 1; // since only using -90->90, convert 0-1 to -1-1
            // oldPitch *= Mathf.Sign(dot);

            var delta = Input.GetAxis("Mouse Y") * lookSpeed * (InvertLookUp ? 1 : -1);
            var newPitch = Mathf.Clamp(pitch + delta, -89, 89);
            camera.transform.Rotate(Vector3.right, newPitch - pitch, Space.Self);

            //camera.transform.localRotation *= 
        }


        // tilt
        {
            var delta = Input.GetAxis("Tilt") * lookSpeed * (InvertLookUp ? 1 : -1);

            if (delta == 0)
                delta = -roll + Mathf.Round(Mathf.Lerp(roll, 0, 0.25f) * 10) / 10;

            var newRoll = Mathf.Clamp(roll + delta, -20, 20);
            camera.transform.Rotate(Vector3.forward, newRoll - roll, Space.Self);
            camera.transform.localPosition = new Vector3(Input.GetAxis("Tilt") * 0.75f, camera.transform.localPosition.y, camera.transform.localPosition.z);
        }
    }

    void Interact(Hand hand)
    {
        if (picker?.Pick != null)
        {
            Interactable[] interactables;
            if ((interactables = picker.Pick.GetComponents<Interactable>()).Length > 0)
            {
                foreach (var act in interactables)
                    act.TryInteract(this, hand);
            }
        }
        else
            hand?.TryUseEquippedItem(this);
    }

    //void OnCollisionEnter(Collision collision)
    //{
    //    isGrounded = Mathf.Abs(Vector3.Dot(rigidbody.velocity, Physics.gravity)) < 0.001f;
    //}

#if DEBUG
    void OnGUI()
    {
        GUI.color = Color.blue;
        //GUI.Label(new Rect(10, 100, 200, 20), $"Grounded: {isGrounded}");
    }
#endif


    void OnDrawGizmos()
    {
        if (camera != null)
            EditorDrawUtils.DrawArrow(3, camera.transform.position, camera.transform.position + camera.transform.forward, camera.transform.up, Color.yellow);
    }
}