using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LockAction
{
    Toggle,
    Lock,
    Unlock,
}

public class Lock : Interactable
{
    public Equippable[] Keys;

    public Behaviour[] Targets;

    public LockAction UseAction;

    public override bool TryInteract(PlayerController player, Hand hand)
    {
        if (hand.EquippedItem == null)
            return false;

        foreach (var key in Keys)
        {
            if (key == hand.EquippedItem)
            {
                UseLock();
                Destroy(hand.EquippedItem.gameObject); // consume key
                hand.EquippedItem = null;
                return true;
            }
        }

        return false;
    }

    void UseLock()
    {
        switch (UseAction)
        {
            case LockAction.Toggle:
                foreach (var target in Targets)
                    target.enabled ^= true;
                break;
            case LockAction.Lock:
                foreach (var target in Targets)
                    target.enabled = false;
                break;
            case LockAction.Unlock:
                foreach (var target in Targets)
                    target.enabled = true;
                break;
        }
    }
}
