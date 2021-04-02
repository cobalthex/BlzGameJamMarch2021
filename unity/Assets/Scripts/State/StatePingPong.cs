using UnityEngine;

public class StatePingPong : MonoBehaviour
{
    public AnalogState State;

    public float DelaySec;

    float nextFlipTime;
    bool waiting;

    void Update()
    {
        if (!waiting && State.Value == State.DesiredValue)
        {
            nextFlipTime = Time.time + DelaySec;
            waiting = true;
        }

        else if (waiting && Time.time >= nextFlipTime)
        {
            waiting = false;
            State.DesiredValue = 1 - State.DesiredValue;
        }
    }
}
