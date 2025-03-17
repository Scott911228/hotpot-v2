using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    public int Need_TargetHP = 10;
    public int Need_DeployCharacterCount = 5;

    void Update()
    {
        CheckAchievements();
    }

    void CheckAchievements()
    {
        if (GameStats.Instance.TargetHP >= Need_TargetHP)
        {
            UnlockAchievement("目標生命值剩餘 X 以上");
        }

        if (GameStats.Instance.DeployedCharacterCount >= Need_DeployCharacterCount)
        {
            UnlockAchievement("派遣 3 個角色以上");
        }

        if (GameStats.Instance.EnemyCount == 0)
        {
            UnlockAchievement("通過關卡");
        }
    }

    void UnlockAchievement(string achievementName)
    {
        Debug.Log("成就達成: " + achievementName);
    }
}
