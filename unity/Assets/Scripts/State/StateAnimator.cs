using UnityEngine;

[ExecuteAlways]
public class StateAnimator : MonoBehaviour
{
    /// <summary>
    /// The state to animate by
    /// </summary>
    public AnalogState State;

    /// <summary>
    /// The origin (pivot point) for rotations. Only localPosition is used
    /// </summary>
    public Transform RotationOrigin;

    /// <summary>
    /// The orientation this transform should be in when <see cref="Value"/> is 0.
    /// This can be at a position other than 0,0,0 - 0°
    /// </summary>
    public Transform OffState;
    /// <summary>
    /// The orientation this transform should be in when <see cref="Value"/> is 1.
    /// </summary>
    public Transform OnState;

    Vector3 rotationOriginPoint;

    void Start()
    {
        rotationOriginPoint = RotationOrigin == null ? Vector3.zero : RotationOrigin.position - transform.position;
    }

    void Update()
    {
        var rotation = Quaternion.Slerp(OffState.localRotation, OnState.localRotation, State.Value);
        var originOffset = -(rotation * rotationOriginPoint) + rotationOriginPoint;
        
        transform.localPosition = Vector3.Lerp(OffState.localPosition, OnState.localPosition, State.Value) + originOffset;
        transform.localRotation = rotation;
        // transform.localScale = Vector3.Lerp(OffState.localScale, OnState.localScale, State.Value); * originScale ?
    }
    void OnDrawGizmos()
    {
        if (RotationOrigin == null)
            return;

        Gizmos.color = new Color(0.45f, 0.3f, 1.0f);
        Gizmos.DrawSphere(RotationOrigin.position, 0.1f);
    }
}