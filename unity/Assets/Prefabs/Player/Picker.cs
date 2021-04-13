using System;
using UnityEngine;

public class Picker : MonoBehaviour
{
    public GUIStyle CtaStyle;
    public GUIStyle CtaShadowStyle;

    public LayerMask LayerMask;
    public float MaxDistance = 1.0f;

    public event EventHandler<RaycastHit> NewPick;

    public Collider Pick { get; private set; } = null;
    private Interactable asInteractable;
    bool wasOutlined;

    public Texture Crosshair;

    // Update is called once per frame
    void Update()
    {
        var transform = Camera.main.transform;
        if (Physics.Raycast(new Ray(transform.position, transform.forward), out var hit, MaxDistance) &&
            ((1 << hit.collider.gameObject.layer) & LayerMask) > 0)
        {
            if (hit.collider != Pick)
            {
                Pick = hit.collider;
                asInteractable = Pick.GetComponent<Interactable>();

                Outline outline = Pick.GetComponent<Outline>();
                if (outline != null)
                {
                    wasOutlined = outline.enabled;
                    outline.enabled = true;
                }

                NewPick?.Invoke(this, hit);
            }
        }
        else
        {
            Outline outline;
            if (Pick != null && (outline = Pick.GetComponent<Outline>()) != null)
                outline.enabled = wasOutlined;

            Pick = null;
        }
    }

    void OnGUI()
    {
        var centerX = Screen.width / 2;
        var centerY = Screen.height / 2;

        if (Pick != null)
        {
            var hintText = string.IsNullOrEmpty(asInteractable?.HintText) 
                ? "" : $"\n<size=15>{(asInteractable.enabled ? asInteractable.HintText : "(disabled)")}</size>";
            var cta = new GUIContent(Pick.name + hintText);
            var size = CtaStyle.CalcSize(cta);
            var pos = new Vector2(centerX - size.x / 2, (Screen.height * .8f) - size.y);
            GUI.Label(new Rect(pos, size), cta, CtaShadowStyle);
            GUI.Label(new Rect(pos, size), cta, CtaStyle);
        }

        if (Crosshair != null)
        {
            var dest = new Rect(
                centerX - Crosshair.width / 2,
                centerY - Crosshair.height / 2,
                Crosshair.width,
                Crosshair.height
            );
            GUI.DrawTexture(dest, Crosshair, ScaleMode.ScaleToFit, true);
        }
    }
}
