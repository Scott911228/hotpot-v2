using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeController : MonoBehaviour
{
    FadeInOut fade;
    void Start()
    {
        fade = FindObjectOfType<FadeInOut>();

        fade.fade_Out();
    }

}
