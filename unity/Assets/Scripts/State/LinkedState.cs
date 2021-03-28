using UnityEngine;

public class LinkedState : MonoBehaviour
{
    public AnalogState Source;
    public float SourceConditionValue = 1; // optional?

    public float DelaySec;
    public AnalogState[] Targets;
    public float TargetValue = 1;
    
    public bool DisableAfterUse = true;

    float activationTime;

    void Update()
    {
        if (activationTime > 0)
        {
            if (Time.time >= activationTime)
            {
                foreach (var target in Targets)
                    target.DesiredValue = TargetValue;

                enabled = !DisableAfterUse;
                activationTime = 0;
            }
            return;
        }

        var value = Source.Value;
        if (value == SourceConditionValue)
            activationTime = Time.time + DelaySec;
    }
}
