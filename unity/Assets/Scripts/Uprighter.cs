using UnityEditor;
using UnityEngine;

public class Uprighter : MonoBehaviour
{
    static Vector3 Multiply(Vector3 a, Vector3 b)
    {
        return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
    }

    void Update()
    {
        var up = -Physics.gravity.normalized;
        var perpUp = Vector3.Cross(new Vector3(up.y, up.z, up.x), up);

        //var similarity = Vector3.Dot(transform.forward, perpUp);
        //var cardinality = Mathf.Sign(Vector3.Dot(up, Vector3.Cross(transform.forward, perpUp)));
        //var angle = Mathf.Acos(similarity) * Mathf.Rad2Deg * cardinality;

        //var desired = Quaternion.AngleAxis(angle, up);
        //transform.rotation = Quaternion.Slerp(transform.rotation, desired, 0.25f);

        var cross = Vector3.Cross(up, perpUp);
        var desired = Quaternion.AngleAxis(0, cross);

        var sign = Mathf.Sign(Vector3.Dot(transform.forward, cross));
        var pitch = Quaternion.Angle(Quaternion.AngleAxis(0, up), transform.rotation) * sign;
        desired *= Quaternion.AngleAxis(pitch, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, desired, 0.25f);
    }
}
