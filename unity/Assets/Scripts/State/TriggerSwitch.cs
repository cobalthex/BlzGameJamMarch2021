using UnityEngine;

public class TriggerSwitch : MonoBehaviour
{
    public bool DisableAfterUse;

    public string TagFilter;

    public Behaviour[] Targets;

    void OnTriggerEnter(Collider collider)
    {
        if (!collider.CompareTag(TagFilter))
            return;

        foreach (var target in Targets)
            target.enabled = true;

        if (DisableAfterUse)
            enabled = false;
    }

    void OnTriggerExit(Collider collider)
    {
        if (!collider.CompareTag(TagFilter))
            return;

        foreach (var target in Targets)
            target.enabled = false;
    }
}
