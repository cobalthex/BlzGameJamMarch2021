using System;
using UnityEngine;

public class Picker : MonoBehaviour
{
    public LayerMask LayerMask;
    public float MaxDistance = 1.0f;

    public event EventHandler<RaycastHit> NewPick;

    public Collider Pick { get; private set; } = null;

    public Texture Crosshair;

    // Update is called once per frame
    void Update()
    {
        var transform = Camera.main.transform;
        if (Physics.Raycast(new Ray(transform.position, transform.forward), out var hit, MaxDistance, LayerMask))
        {
            if (hit.collider != Pick)
            {
                Pick = hit.collider;
                NewPick?.Invoke(this, hit);
            }
        }
        else
            Pick = null;
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 100, 20), $"{Pick}");

        if (Crosshair != null)
        {
            var dest = new Rect(
                (Screen.width - Crosshair.width) / 2,
                (Screen.height - Crosshair.height) / 2,
                Crosshair.width,
                Crosshair.height
            );
            GUI.DrawTexture(dest, Crosshair, ScaleMode.ScaleToFit, true);
        }
    }
}
