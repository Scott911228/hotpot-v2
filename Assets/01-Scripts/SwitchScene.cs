using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SwitchScene : MonoBehaviour
{
    public string loadSceneName;
    FadeInOut fade;

    void Start() 
    {
        fade = FindObjectOfType<FadeInOut>();
    }

    public void SwitchSceneTo(string loadSceneName)
    {
        SceneManager.LoadScene(loadSceneName);
    }


    public IEnumerator FadeToSceneNoWait()
    {
        fade.fade_In();
        yield return new WaitForSeconds(0);
        SceneManager.LoadScene(loadSceneName);
    }
    public IEnumerator FadeToScene()
    {
        fade.fade_In();
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(loadSceneName);
    }
    public void FadeToScene(string loadSceneName)
    {
        StartCoroutine(FadeToScene());
    }    
    public void FadeToSceneNoWait(string loadSceneName)
    {
        StartCoroutine(FadeToSceneNoWait());
    }
}
