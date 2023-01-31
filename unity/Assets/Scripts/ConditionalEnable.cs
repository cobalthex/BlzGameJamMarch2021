using UnityEngine;
public enum ConditionRule
{
    Equal,
    NotEqual,
    Less,
    Greater,
}

public enum ConditionCombination
{
    And,
    Or,
}

[System.Serializable]
public struct Condition
{
    public AnalogState State;
    public float Value;
    public ConditionRule Rule;

    public bool Test()
    {
        return Rule switch
        {
            ConditionRule.Equal     => (Value == State.Value),
            ConditionRule.NotEqual  => (Value != State.Value),
            ConditionRule.Less      => (Value < State.Value),
            ConditionRule.Greater   => (Value > State.Value),
            _ => false,
        };
    }
}


[ExecuteAlways]
public class ConditionalEnable : MonoBehaviour
{

    public ConditionCombination Combination;
    public Condition[] Conditions;

    bool lastState;

    public bool DisableAfterUse;

    /// <summary>
    /// Targets to enable or disable based on whether or not the condition is satisfied
    /// </summary>
    public Behaviour[] Targets;

    void Update()
    {
        if (Conditions == null)
            return;

        int test = 0;
        foreach (var cond in Conditions)
        {
            if (cond.State == null || Targets == null)
                return;

            test += cond.Test() ? 1 : 0;
        }

        bool state
            = Combination == ConditionCombination.And
            ? (test == Conditions.Length)
            : (test > 0);

        if (state != lastState)
        {
            lastState = state;
            foreach (var target in Targets)
                target.enabled = state;

            enabled = !DisableAfterUse;
        }
    }
}
