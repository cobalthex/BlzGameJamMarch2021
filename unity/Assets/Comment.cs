using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Comment : MonoBehaviour
{
    [Multiline]
    public string Text = "[Enter comment]";

    public GUIStyle Style;

    private void Start()
    {
        Style ??= GUI.skin.box;
    }

    private void OnDrawGizmos()
    {
        var camera = SceneView.currentDrawingSceneView.camera;

        // todo: draw box

        // use ContentOffset to offset text
        Handles.Label(transform.position, Text, Style);
    }
}