using UnityEngine;
public enum ConditionRule
{
    Equal,
    NotEqual,
    Less,
    Greater,
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
    public Condition Condition;

    bool lastState;

    public bool DisableAfterUse;

    /// <summary>
    /// Targets to enable or disable based on whether or not the condition is satisfied
    /// </summary>
    public Behaviour[] Targets;

    void Update()
    {
        if (Condition.State == null || Targets == null)
            return;

        var state = Condition.Test();

        if (state != lastState)
        {
            lastState = state;
            foreach (var target in Targets)
                target.enabled = state;

            enabled = !DisableAfterUse;
        }
    }
}
