using UnityEngine;

[ExecuteAlways]
public class AnalogState : MonoBehaviour
{
    /// <summary>
    /// The value of this state, clamped from 0 to 1
    /// </summary>
    [Range(0, 1)]
    public float DesiredValue = 0;

    public float Value { get; set; }

    /// <summary>
    /// How quickly to change the value, per second
    /// </summary>
    /// <remarks>Come up with a better name?</remarks>
    public float Speed = 1;

    public float Duration => 1 / Speed;

    void Update()
    {
        if (Application.isPlaying)
        {
            var dist = DesiredValue - Value;
            if (dist == 0)
                return;

            Value += Mathf.Sign(dist) * Mathf.Min(Mathf.Abs(dist), Time.deltaTime * Speed); // use coroutine?
        }
        else
            Value = DesiredValue;
    }
}
