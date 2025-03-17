using UnityEngine;

public class GameStats : MonoBehaviour
{
    public static GameStats Instance;

    public int TargetHP;
    public int DeployedCharacterCount;
    public int EnemyCount;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    public void CheckAchievements()
    {
        if (DeployedCharacterCount >= 3) // 假設達成成就是派遣 10 名角色
        {
            UnlockAchievement("派遣三名角色");
        }

        if (TargetHP >= 3) // 假設達成成就是剩餘3生命值以上
        {
            UnlockAchievement("生命剩餘大於三");
        }
    }

    public void UnlockAchievement(string message)
    {
        // 顯示成就 UI
        Debug.Log("成就達成: " + message);
    }
}
