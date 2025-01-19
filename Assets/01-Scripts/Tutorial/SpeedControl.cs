using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedControl : MonoBehaviour
{
    public Animator animator;
    public GameObject PauseMenu;
    public float TimeScale = 1f;
    public Button Button;
    public Sprite Speed1X;
    public Sprite Speed2X;
    public bool isForceNoSpeed;
    public bool isForceSlowdown;
    // Start is called before the first frame update
    void Start()
    {
        //animator = GameObject.Find("FloatTipsBase").GetComponent<Animator>();
    }
    public void SwitchSpeed()
    {
        if (TimeScale == 1f)
        {
            TimeScale = 2f;
            Button.GetComponent<Image>().sprite = Speed2X;
        }
        else
        {
            TimeScale = 1f;
            Button.GetComponent<Image>().sprite = Speed1X;
        }
        ChangeSprite();
    }
    public void ChangeSprite()
    {
        if (TimeScale == 1f)
        {
            Button.GetComponent<Image>().sprite = Speed1X;
        }
        else
        {
            Button.GetComponent<Image>().sprite = Speed2X;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (PauseMenu.GetComponent<PauseMenu>().UI.activeSelf) Time.timeScale = 0f;
        else if (isForceNoSpeed) Time.timeScale = 1f;
        else if (isForceSlowdown) Time.timeScale = 0.05f;
        else Time.timeScale = TimeScale;

        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("FloatTips");
        foreach (GameObject gameObject in gameObjects)
        {
            Animator animator = gameObject.GetComponent<Animator>();
            if (animator.gameObject.activeSelf)
            {
                animator.SetFloat("runMultiplier", 1 / Time.timeScale);
            }
        }
    }
}
