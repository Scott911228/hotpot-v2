using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
        roundsText.text = GameObject.Find("GameControl").GetComponent<PlayerStats>().Rounds.ToString();
        //myAnimator.SetTrigger("");
    }

    public void Retry()
    {
        SceneManager.LoadScene( SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1f;
    }

    public void Continue()
    {
        Time.timeScale = 1f;
        UI.SetActive(false);
    }
}
