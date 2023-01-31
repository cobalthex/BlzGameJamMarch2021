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
    public Equippable[] AnyOfTheseKeys;
    public bool ConsumeKey = true;

    public Behaviour[] Targets;

    public LockAction UseAction;

    public override bool TryInteract(PlayerController player, Hand hand)
    {
        if (hand.EquippedItem == null)
            return false;

        foreach (var key in AnyOfTheseKeys)
        {
            if (key == hand.EquippedItem)
            {
                UseLock();
                if (ConsumeKey)
                {
                    Destroy(hand.EquippedItem.gameObject); // consume key
                    hand.EquippedItem = null;
                }
                return true;
            }
        }

        return false;
    }

    public void ForceUseLock()
    {
        UseLock();
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
        StartCoroutine(AnimateLockCoroutine());
    }

    IEnumerator AnimateLockCoroutine()
    {
        var startingPosition = transform.localPosition;

        for (float i = 0; i <= 2; i += 0.2f)
        {
            var amt = Mathf.PingPong(i, 1);
            transform.localPosition = startingPosition + new Vector3(0, 0, -amt * 0.05f);
            yield return null;
        }
        transform.localPosition = startingPosition;
    }
}
