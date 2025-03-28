using UnityEngine;

public class GameStats : MonoBehaviour
{
    public static GameStats Instance;

    public int TargetHP => LevelSettings.Instance != null ? LevelSettings.Instance.Life : 0;
    public int DeployedCharacterCount;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
}
