using UnityEngine;

public class LinkedState : MonoBehaviour
{
    public Condition Source;

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

        if (Source.Test())
            activationTime = Time.time + DelaySec;
    }
}
