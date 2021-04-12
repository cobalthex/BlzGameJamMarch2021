using System.Collections;
using UnityEngine;

public class ScreenFade : MonoBehaviour
{
    public Color StartColor = Color.white;
    public Color EndColor = new Color(1, 1, 1, 0);

    public float DurationSec = 2;

    public bool DisableAfter = true;

    float startTime;
    Color currentColor;

    static Texture2D tex;

    private void Awake()
    {
        if (tex == null)
        {
            tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, Color.white);
        }
    }

    private void OnEnable()
    {
        startTime = Time.time;
        StartCoroutine(Fade(DurationSec));
    }

    IEnumerator Fade(float durationSec)
    {
        while (Time.time < startTime + DurationSec)
        {
            currentColor = Color.Lerp(StartColor, EndColor, (Time.time - startTime) / DurationSec);
            yield return null;
        }

        enabled = !DisableAfter;
    }


    private void OnGUI()
    {
        Graphics.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), tex, new Rect(0, 0, 1, 1), 0, 0, 0, 0, currentColor);
    }
}
