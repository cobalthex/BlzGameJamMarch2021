using UnityEngine;

public class Compass : MonoBehaviour
{
    public Vector3 North = Vector3.forward;

    void LateUpdate()
    {
        var perpNorth = Vector3.Cross(new Vector3(North.y, North.z, North.x), North);

        var similarity = Vector3.Dot(transform.forward, North);
        var cardinality = -Mathf.Sign(Vector3.Dot(North, Vector3.Cross(transform.forward, perpNorth)));
        var angle = Mathf.Acos(similarity) * Mathf.Rad2Deg;

        var desired = Mathf.Lerp(angle, 0, 0.25f);
        transform.localRotation = Quaternion.AngleAxis(desired, Vector3.up);
    }
}
