using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject UI;
    public GameObject SayDialog;
    public SpeedControl speedControl;
    public string characterTag = "Character";
    public string enemyTag = "Enemy";

    public void test()
    {
        Debug.Log("Standard Tuuret Selected");
    }

    void Start()
    {
        speedControl = GameObject.Find("SpeedControl").GetComponent<SpeedControl>();
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            Toggle();
        }
    }

    public void Toggle()
    {
        UI.SetActive(!UI.activeSelf);
        if(SayDialog != null) SayDialog.SetActive(!SayDialog.activeSelf);

        if (UI.activeSelf)
        {
            GamePause();
        }
        else
        {
            GameResume();
        }
    }
    public void GamePause()
    {
        GameObject.Find("GameControl").GetComponent<GameManager>().isGamePlaying = false;
        speedControl.GetComponent<SpeedControl>().isForceNoSpeed = true;
        GameObject[] Enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        GameObject[] Characters = GameObject.FindGameObjectsWithTag(characterTag);

        foreach (GameObject Enemy in Enemies)
        {
            Enemy.GetComponent<Enemies>().isPaused = true;
            Enemy.GetComponent<EnemyAttack>().isPaused = true;
        }
        foreach (GameObject Character in Characters)
        {
            Character.GetComponent<Character>().isPaused = true;
        }
    }
    public void GameResume()
    {
        GameObject.Find("GameControl").GetComponent<GameManager>().isGamePlaying = true;
        speedControl.GetComponent<SpeedControl>().isForceNoSpeed = false;
        GameObject[] Enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        GameObject[] Characters = GameObject.FindGameObjectsWithTag(characterTag);

        foreach (GameObject Enemy in Enemies)
        {
            Enemy.GetComponent<Enemies>().isPaused = false;
            Enemy.GetComponent<EnemyAttack>().isPaused = false;
        }
        foreach (GameObject Character in Characters)
        {
            Character.GetComponent<Character>().isPaused = false;
        }
    }
    public void Retry()
    {

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }
}
