using UnityEditor;
using UnityEngine;

public class Compass : MonoBehaviour
{
    public Vector3 North = Vector3.forward;

    private void Start()
    {
    }

    void Update()
    {
        var perpNorth = Vector3.Cross(new Vector3(North.y, North.z, North.x), North);

        var similarity = Vector3.Dot(transform.forward, North);
        var cardinality = Mathf.Sign(Vector3.Dot(North, Vector3.Cross(transform.forward, perpNorth))); // -/+1 based on if right or left of forwards
        
        // fuck math

        var lerp = Mathf.Lerp(similarity, 1, 0.25f) * cardinality;
        var desired = Mathf.Acos(lerp) * Mathf.Rad2Deg;

        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, desired, transform.localEulerAngles.z);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        EditorDrawUtils.DrawArrow(1f, transform.position, transform.position + North * 0.5f, transform.up, Color.red);

        var perpNorth = Vector3.Cross(new Vector3(North.y, North.z, North.x), North);

        var similarity = Vector3.Dot(transform.forward, North);
        var cardinality = Mathf.Sign(Vector3.Dot(North, Vector3.Cross(transform.forward, perpNorth)));

        var lerp = Mathf.Lerp(similarity, 1, 0.25f) * cardinality;
        var desired = Mathf.Acos(lerp) * Mathf.Rad2Deg;

        var text = $"{similarity}\n{cardinality}\n{lerp}\n{desired}";
        Handles.Label(transform.position + new Vector3(0.2f, 0.2f, 0), text);
    }
#endif
}
