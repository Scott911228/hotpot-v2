using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneChanger : MonoBehaviour
{

    FadeInOut fade;

    void Start() 
    {
        fade = FindObjectOfType<FadeInOut>();
    }

    public IEnumerator FadeToTutorial()
    {
        fade.fade_In();
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene("Tutorial");
    }

    public IEnumerator FadeToPrepareScene()
    {
        fade.fade_In();
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene("PrepareScene");
    }

    public void ToTutorial()
    {
        StartCoroutine(FadeToTutorial());
    }

    public void ToPrepareScene()
    {
        StartCoroutine(FadeToPrepareScene());
    }

    public void ToTitleScene()
    {
        SceneManager.LoadScene("TitleScene");
    }

    public void ToBeginningScene()
    {
        SceneManager.LoadScene("Beginning");
    }
    
}