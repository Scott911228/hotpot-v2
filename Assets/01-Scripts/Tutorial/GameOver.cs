﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOver : MonoBehaviour
{
    public Text roundsText;
    public GameObject UI;
    Animator myAnimator;

    void Start()
    {
        myAnimator = GetComponent<Animator>();
        Time.timeScale = 0;
    }
    void Awake()
    {
        //myAnimator.SetTrigger("");
    }

    public void Retry()
    {
        GameManager.isRestarted = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1f;
    }

    public void Continue()
    {
        Time.timeScale = 1f;
        UI.SetActive(false);
    }
}
