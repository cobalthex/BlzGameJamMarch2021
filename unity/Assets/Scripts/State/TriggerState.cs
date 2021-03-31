using UnityEngine;

public class TriggerState : MonoBehaviour
{
    public string TagFilter;

    public AnalogState LinkedState;
    public float DesiredValue = 1;
    
    public bool DisableAfterUse;

    bool didUse = false;

    private void OnTriggerStay(Collider other)
    {
        if (didUse || !enabled)
            return;

        if (!other.CompareTag(TagFilter))
            return;

        didUse = true;

        LinkedState.DesiredValue = DesiredValue;

        if (DisableAfterUse)
            enabled = false;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!enabled ||
            !GetComponent<Collider>().CompareTag(TagFilter))
            return;

        didUse = false;
    }
}
