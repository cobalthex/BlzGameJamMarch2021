using UnityEngine;

public class Uprighter : MonoBehaviour
{
    void Update()
    {
        var up = -Physics.gravity.normalized;

        var cross = Vector3.Cross(up, Vector3.right); // not sure if this works if gravity is not down
        var desired = Quaternion.AngleAxis(0, cross);

        var sign = Mathf.Sign(Vector3.Dot(transform.forward, Vector3.right));
        var pitch = Quaternion.Angle(Quaternion.AngleAxis(0, up), transform.rotation) * sign;
        desired *= Quaternion.AngleAxis(pitch, Vector3.up);

        transform.rotation = Quaternion.Slerp(transform.rotation, desired, 0.25f);
    }
}
