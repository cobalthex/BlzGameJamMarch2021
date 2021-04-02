using UnityEngine;

public abstract class Equippable : Interactable
{
    public Transform RelativeTransform;

    public override bool TryInteract(PlayerController player, Hand hand)
    {
        hand.EquippedItem = this;
        return true;
    }

    public virtual void Use(PlayerController player) { }
}
