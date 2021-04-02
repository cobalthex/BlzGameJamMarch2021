using UnityEngine;

public class StateLooper : MonoBehaviour
{
    public AnalogState State;

    void Update()
    {
        if (State.Value == State.DesiredValue)
            State.Value = 1 - State.DesiredValue;
    }
}
