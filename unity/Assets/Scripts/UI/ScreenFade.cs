using UnityEngine;

public class ScreenFade : MonoBehaviour
{
    public Color StartColor = Color.white;
    public Color EndColor = new Color(1, 1, 1, 0);

    public float Duration = 2;

    float startTime;

    static Texture2D tex;

    private void Start()
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
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > startTime + Duration)
            enabled = false;
    }

    private void OnGUI()
    {
        var c = Color.Lerp(StartColor, EndColor, (Time.time - startTime) / Duration);
        Graphics.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), tex, new Rect(0, 0, 1, 1), 0, 0, 0, 0, c);
    }
}
