using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayFps : MonoBehaviour
{
    void OnGUI()
    {
        float fps = 1.0f / Time.deltaTime;
        GUI.Label(new Rect(10, 10, 100, 20), $"FPS: {fps}");
    }
}
