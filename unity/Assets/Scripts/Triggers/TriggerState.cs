using UnityEngine;

public class TriggerState : MonoBehaviour
{
    public string TagFilter;

    public AnalogState LinkedState;
    public float DesiredValue = 1;

    public bool Continuous;
    public bool DisableAfterUse;

    bool didUse = false;

    private void OnTriggerStay(Collider other)
    {
        if ((!Continuous && didUse) || !enabled)
            return;

        if (TagFilter != null && !other.CompareTag(TagFilter))
            return;

        didUse = true;

        LinkedState.DesiredValue = DesiredValue;

        if (DisableAfterUse)
            enabled = false;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!enabled ||
            (TagFilter != null && !other.CompareTag(TagFilter)))
            return;

        didUse = false;
    }
}
