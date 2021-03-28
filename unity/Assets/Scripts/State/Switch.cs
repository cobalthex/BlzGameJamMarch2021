using UnityEngine;

public enum SwitchState
{
    Off,
    On
}

[ExecuteAlways]
public class Switch : MonoBehaviour
{
    /// <summary>
    /// A state to update when this switch is triggered
    /// </summary>
    public AnalogState LinkedState;

    /// <summary>
    /// Reset this switch back to its default delay after this amount of time.
    /// (This delay is after animations finish).
    /// Use a negative value for never
    /// </summary>
    public float ResetDelaySec;

    public bool DisableAfterUse;
    public bool CanOnlyUseInDefaultState;

    public SwitchState DefaultState = SwitchState.Off;

    public SwitchState State
    {
        get => _state;
        set
        {
            if (_state == value || Time.time < nextActivationTime)
                return;

            _state = value;

            if (DisableAfterUse)
                enabled = false;

            if (LinkedState != null)
            {
                LinkedState.DesiredValue = (value == SwitchState.Off ? 0 : 1);
                nextActivationTime = Time.time + LinkedState.Duration;
            }

            if (value != DefaultState)
                nextResetTime = nextActivationTime + ResetDelaySec;
        }
    }
    private SwitchState _state;

    float nextActivationTime;
    float nextResetTime;

    void Awake()
    {
        State = DefaultState;
        if (LinkedState != null)
            LinkedState.Value = LinkedState.DesiredValue = (State == SwitchState.Off ? 0 : 1);
    }

    void Update()
    {
        if (ResetDelaySec >= 0 && Time.time >= nextResetTime)
        {
            State = DefaultState;
        }
    }

    public void Flip()
    {
        if (!enabled ||
            (CanOnlyUseInDefaultState && State != DefaultState))
            return;

        switch (State)
        {
            case SwitchState.Off:
                State = SwitchState.On;
                break;
            case SwitchState.On:
                State = SwitchState.Off;
                break;
        }
    }

    public override string ToString()
    {
        return base.ToString() + $" ({State})";
    }

    void OnDrawGizmos()
    {
        switch (State)
        {
            case SwitchState.Off:
                Gizmos.color = Color.red;
                break;
            case SwitchState.On:
                Gizmos.color = Color.green;
                break;
        }

        if (Time.time < nextActivationTime ||
            (CanOnlyUseInDefaultState && State != DefaultState))
            Gizmos.color = Color.yellow;

        if (!enabled)
            Gizmos.color = Color.gray;

        Gizmos.DrawCube(transform.position + transform.up * 0.5f, new Vector3(0.1f, 0.1f, 0.1f));
    }
}
