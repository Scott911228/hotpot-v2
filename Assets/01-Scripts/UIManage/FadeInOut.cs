using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeInOut : MonoBehaviour
{

    public CanvasGroup canvasGroup;
    public bool fadeIn = false;
    public bool fadeOut = false;

    public float TimeToFade;
    float counter = 0f;

    void Update()
    {
        if (fadeIn == true)
        {
            if (canvasGroup.alpha < 1)
            {
                counter += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(0, 1, counter / TimeToFade);
                if (counter >= TimeToFade)
                {
                    fadeIn = false;
                    fadeOut = false;
                    counter = 0f;
                }
            }
        }

        if (fadeOut == true)
        {
            if (canvasGroup.alpha >= 0)
            {
                counter += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(1, 0, counter / TimeToFade);
                if (counter >= TimeToFade)
                {
                    fadeIn = false;
                    fadeOut = false;
                    counter = 0f;
                }
            }
        }
    }

    public void fade_In()
    {
        fadeIn = true;
    }

    public void fade_Out()
    {
        fadeOut = true;
    }


}
