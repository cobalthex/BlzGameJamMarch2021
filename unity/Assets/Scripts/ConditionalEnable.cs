using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class ConditionalEnable : MonoBehaviour
{
    public AnalogState Condition;
    public float ConditionValue; // ge, le, eq?

    bool lastState;

    public bool DisableAfterUse;

    /// <summary>
    /// Targets to enable or disable based on whether or not the condition is satisfied
    /// </summary>
    public Behaviour[] Targets;

    void Update()
    {
        if (Condition == null || Targets == null)
            return;

        // initial state?
        var state = Condition.Value == ConditionValue;
        if (state != lastState)
        {
            lastState = state;
            foreach (var target in Targets)
                target.enabled = state;

            enabled = !DisableAfterUse;
        }
    }
}
