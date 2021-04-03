using UnityEngine;

public class ScreenFadeIn : MonoBehaviour
{
    public Color Color = Color.white;

    public float Duration = 2;

    float startTime;

    static Texture2D tex;

    private void Start()
    {
        startTime = Time.time;

        if (tex == null)
        {
            tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, Color.white);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > startTime + Duration)
            enabled = false;
    }

    private void OnGUI()
    {
        var a = Mathf.Lerp(1, 0, (Time.time - startTime) / Duration);
        Graphics.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), tex, new Rect(0, 0, 1, 1), 0, 0, 0, 0, new Color(Color.r, Color.g, Color.a, a));
    }
}
