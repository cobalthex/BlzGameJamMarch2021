using UnityEngine;

public class Flashlight : Equippable
{
    GameObject light;

    void Start()
    {
        light = transform.Find("light").gameObject;
    }

    public override void Use(PlayerController player)
    {
        light.SetActive(light.activeSelf ^ true);
    }
}
