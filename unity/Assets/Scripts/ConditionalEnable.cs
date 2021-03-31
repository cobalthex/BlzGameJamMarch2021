using System.Collections.Generic;
using UnityEngine;

public enum ConditionRule
{
    Equal,
    NotEqual,
    Less,
    Greater,
}

[ExecuteAlways]
public class ConditionalEnable : MonoBehaviour
{
    public AnalogState Condition;
    public float ConditionValue;
    public ConditionRule ConditionRule;

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

        var state = ConditionRule switch
        {
            ConditionRule.Equal     => (Condition.Value == ConditionValue),
            ConditionRule.NotEqual  => (Condition.Value != ConditionValue),
            ConditionRule.Less      => (Condition.Value < ConditionValue),
            ConditionRule.Greater   => (Condition.Value > ConditionValue),
            _ => false,
        };

        if (state != lastState)
        {
            lastState = state;
            foreach (var target in Targets)
                target.enabled = state;

            enabled = !DisableAfterUse;
        }
    }
}
