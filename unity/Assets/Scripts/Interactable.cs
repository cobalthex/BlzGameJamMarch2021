using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public abstract bool TryInteract(PlayerController player, Hand hand);
}
