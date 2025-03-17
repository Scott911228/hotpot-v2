using UnityEngine;

public class GameStats : MonoBehaviour
{
    public static GameStats Instance;

    public int TargetHP => LevelSettings.Instance != null ? LevelSettings.Instance.Life : 0;
    public int DeployedCharacterCount;
    public int RemainingEnemyCount { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    // 更新敵人數量
    public void AddEnemy()
    {
        RemainingEnemyCount++;
    }

    public void RemoveEnemy()
    {
        RemainingEnemyCount--;

        // 若敵人數量歸零，觸發遊戲結束
        if (RemainingEnemyCount <= 0)
        {
            EndGame();
        }
    }

    private void EndGame()
    {
        Debug.Log("所有敵人已消滅，遊戲結束");
        AchievementManager.Instance.CheckAchievements(); // 遊戲結束時觸發成就檢測
    }
}
