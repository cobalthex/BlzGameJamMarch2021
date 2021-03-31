using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : Equippable
{
    Light light;

    void Start()
    {
        light = GetComponentInChildren<Light>();
    }

    public override void Use(PlayerController player)
    {
        light.enabled ^= true;
    }
}
