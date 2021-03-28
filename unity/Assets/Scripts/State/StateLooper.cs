using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateLooper : MonoBehaviour
{
    public AnalogState State;

    void Start()
    {
        State ??= GetComponent<AnalogState>();
    }

    void Update()
    {
        if (State.Value == State.DesiredValue)
            State.Value = 1 - State.DesiredValue;
    }
}
