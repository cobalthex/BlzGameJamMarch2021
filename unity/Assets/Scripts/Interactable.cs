using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public string HintText;

    private void Start()
    {
        gameObject.layer = LayerMask.NameToLayer(nameof(Interactable));

        if (GetComponent<Collider>() == null)
            throw new MissingComponentException("Interactables require a collider to find");
    }

    public abstract bool TryInteract(PlayerController player, Hand hand);
}
