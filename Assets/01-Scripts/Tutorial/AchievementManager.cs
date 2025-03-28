using UnityEngine;

public class AchievementManager : MonoBehaviour
{   
    public static AchievementManager Instance; // 新增 Singleton 讓其他地方可以直接呼叫

    public int Need_TargetHP = 3;
    public int Need_DeployCharacterCount = 5;

    private bool isTargetHpAchieved = false;
    private bool isDeployAchieved = false;
    private bool isClearAchieved = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void CheckAchievements()
    {
        // 生命值成就檢測
        if (GameStats.Instance.TargetHP >= Need_TargetHP && !isTargetHpAchieved)
        {
            isTargetHpAchieved = true;
            UnlockAchievement("目標生命值剩餘 3 以上");
        }

        // 派遣角色成就檢測
        if (GameStats.Instance.DeployedCharacterCount >= Need_DeployCharacterCount && !isDeployAchieved)
        {
            isDeployAchieved = true;
            UnlockAchievement("派遣 3 個小高以上");
        }

        // 通過關卡成就檢測
        if (true)
        {
            isClearAchieved = true;
            UnlockAchievement("通過關卡");
        }
    }

    void UnlockAchievement(string achievementName)
    {
        Debug.Log("成就達成: " + achievementName);
    }
}
