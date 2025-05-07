using UnityEngine;

public class GameQuitter : MonoBehaviour
{
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("QuitGame called");
    }
}