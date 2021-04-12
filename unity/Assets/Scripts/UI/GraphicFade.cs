using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GraphicFade : MonoBehaviour
{
    public Color StartColor = Color.white;
    public Color EndColor = new Color(1, 1, 1, 0);

    public float DurationSec = 2;

    public bool DisableAfter = true;

    float startTime;
    Color currentColor;
    Graphic graphic;

    private void Awake()
    {
        graphic = GetComponent<Graphic>();
    }

    private void OnEnable()
    {
        startTime = Time.time;
        graphic.enabled = true;
        StartCoroutine(Fade(DurationSec));
    }

    IEnumerator Fade(float durationSec)
    {
        while (Time.time < startTime + DurationSec)
        {
            graphic.color = Color.Lerp(StartColor, EndColor, (Time.time - startTime) / DurationSec);
            yield return null;
        }

        graphic.enabled = enabled = !DisableAfter;
    }
}
