using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Television : MonoBehaviour
{
    public Camera LinkedCamera;

    void Start()
    {
        var display = GetComponent<Renderer>();

        if (LinkedCamera.targetTexture == null)
        {
            LinkedCamera.targetTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.DefaultHDR)
            {
                useDynamicScale = true
            };
        }

        display.material.mainTexture = LinkedCamera.targetTexture;
    }
}
