using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{

    public void LoadingLevels(string nameOftheLevel)
    {
        SceneManager.LoadScene(nameOftheLevel);
    }
}