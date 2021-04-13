using UnityEngine;

public class Equippable : Interactable
{
    public Transform RelativeTransform;

    public override bool TryInteract(PlayerController player, Hand hand)
    {
        Outline outline;
        if ((outline = GetComponent<Outline>()) != null)
            outline.enabled = false;

        hand.EquippedItem = this;
        return true;
    }

    public virtual void Use(PlayerController player) { }
}
