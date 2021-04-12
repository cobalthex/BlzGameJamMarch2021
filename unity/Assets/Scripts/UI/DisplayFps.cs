using UnityEngine;

public class DisplayFps : MonoBehaviour
{
    public float MinimumFps = 30;
    public float OptimalFps = 60;

    GUIStyle style;

    void Start()
    {
        style = new GUIStyle
        {
            alignment = TextAnchor.UpperRight,
        };

#if !DEBUG
        enabled = false;
#endif
    }

    void OnGUI()
    {
        float fps = 1f / Time.deltaTime;

        if (fps >= OptimalFps)
            style.normal.textColor = Color.black;
        else if (fps >= MinimumFps)
            style.normal.textColor = Color.yellow;
        else
            style.normal.textColor = Color.red;
            
        GUI.Label(new Rect(5, 5, Screen.width - 10, Screen.height - 10), $"FPS: {fps:N1}", style);
    }
}
