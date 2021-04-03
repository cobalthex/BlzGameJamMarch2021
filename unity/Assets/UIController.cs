using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{

    public int fadeSpeed = 3;

    private Image screenImage;

    void Start()
    {
        screenImage = GetComponentInChildren<Image>();
    }

    void gameOver()
    {
        StartCoroutine(fadeScreen(fadeSpeed));
    }

    IEnumerator fadeScreen(int fadeSpeed = 3, bool fadeOut = true)
    {
        float fadeAmount = 0.0f;

        Color imageColor = screenImage.color;
        if (fadeOut)
        {
            while (screenImage.color.a < 1)
            {
                fadeAmount = imageColor.a + fadeSpeed * Time.deltaTime;
                imageColor = new Color(imageColor.r, imageColor.g, imageColor.b, fadeAmount);
                screenImage.color = imageColor;
                yield return null;
            }
        }
        else
        {
            while (screenImage.color.a > 0)
            {
                fadeAmount = imageColor.a - (fadeSpeed * Time.deltaTime);
                imageColor = new Color(imageColor.r, imageColor.g, imageColor.b, fadeAmount);
                screenImage.color = imageColor;
                yield return null;
            }
        }
    }
}
