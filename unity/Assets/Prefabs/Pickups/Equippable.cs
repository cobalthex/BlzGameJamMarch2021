using UnityEngine;

public abstract class Equippable : MonoBehaviour
{
    public Transform RelativeTransform;

    public virtual void Use(PlayerController player) { }
}
