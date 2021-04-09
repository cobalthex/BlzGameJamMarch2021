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

    public bool UseEulerRotation = false;

    public float Steps = 0;

    Vector3 rotationOriginPoint;

    void Start()
    {
        rotationOriginPoint = RotationOrigin == null ? Vector3.zero : RotationOrigin.position - transform.position; // local position?
    }

    void Update()
    {
        var stateValue = State.Value;
        if (Steps > 0)
            stateValue = Mathf.Round(State.Value * Steps) / Steps;

        Quaternion rotation;
        if (UseEulerRotation)
            rotation = Quaternion.Euler(Vector3.Lerp(OffState.localEulerAngles, OnState.localEulerAngles, stateValue)); //not sure how this affects positioning
        else
            rotation = Quaternion.Slerp(OffState.localRotation, OnState.localRotation, stateValue);

        var originOffset = -(rotation * rotationOriginPoint) + rotationOriginPoint;
        
        transform.localPosition = Vector3.Lerp(OffState.localPosition, OnState.localPosition, stateValue) + originOffset;
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