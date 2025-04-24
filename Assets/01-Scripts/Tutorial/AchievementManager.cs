using UnityEngine;
using System.Collections.Generic;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance; // Singleton

    public HashSet<int> AchievementGranted = new HashSet<int>(); // 使用 HashSet 儲存成就
    private AchievementSet[] AchievementSets;

    // 使用 Dictionary 儲存不同的成就判定方式
    private Dictionary<AchievementSet.AchievementValueJudgeEnum, System.Func<float, float, bool>> judgeActions;

    void Start()
    {
        //InvokeRepeating("CheckAchievements", 0f, 0.5f);
    }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 保證在場景切換時不會銷毀
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        InitializeJudgeActions();
    }

    // 初始化不同的判定邏輯
    private void InitializeJudgeActions()
    {
        judgeActions = new Dictionary<AchievementSet.AchievementValueJudgeEnum, System.Func<float, float, bool>>
        {
            { AchievementSet.AchievementValueJudgeEnum.Above, (current, target) => current >= target },
            { AchievementSet.AchievementValueJudgeEnum.Equal, (current, target) => Mathf.Approximately(current, target) },
            { AchievementSet.AchievementValueJudgeEnum.Below, (current, target) => current < target },
            { AchievementSet.AchievementValueJudgeEnum.None, (current, target) => false }
        };
    }

    public void CheckAchievements()
    {
        bool isGameOver = GameManager.isGameOver;
        if (isGameOver) return;
        AchievementSets = GameObject.Find("LevelSettings").GetComponent<LevelSettings>().AchievementSets;
        int AchievementIndex = 0;
        foreach (var achievement in AchievementSets)
        {
            // 檢查是否已解鎖該成就
            if (AchievementGranted.Contains(AchievementIndex))
            {
                AchievementIndex++;
                continue;
            }
            float currentValue = GetAchievementValue(achievement);
            // 確保有正確的判定邏輯
            if (judgeActions.TryGetValue(achievement.AchievementValueJudge, out var judgeFunction))
            {
                float targetValue = achievement.value;
                Debug.Log($"檢查成就 [{AchievementIndex}] - 類型: {achievement.AchievementType}, 判定: {achievement.AchievementValueJudge}, 目標值: {targetValue}, 當前值: {currentValue}");

                if (judgeFunction(currentValue, targetValue))
                {
                    UnlockAchievement(AchievementIndex);
                }
            }
            else
            {
                Debug.LogError($"未知的判定方式: {achievement.AchievementValueJudge}");
            }
            AchievementIndex++;
        }
    }

    // 根據成就類型獲取對應的數值
    private float GetAchievementValue(AchievementSet achievement)
    {
        switch (achievement.AchievementType)
        {
            case AchievementSet.AchievementTypeEnum.WinLevel:
                return GameStats.Instance.LevelCleared ? 1f : 0f;
            case AchievementSet.AchievementTypeEnum.KillCount:
                return GameStats.Instance.EnemyKilledCount;
            case AchievementSet.AchievementTypeEnum.BaseHealth:
                return PlayerStats.Life;
            case AchievementSet.AchievementTypeEnum.DispatchTotalCount:
                return GameStats.Instance.DeployedCharacterCount;
            case AchievementSet.AchievementTypeEnum.DispatchSpecificCount:
                return GameStats.Instance.GetCharacterDispatchCount(achievement.character);
            case AchievementSet.AchievementTypeEnum.SurviveTime:
                if (LevelSettings.Instance.LevelType == LevelType.LevelTypeEnum.Time)
                    return GameStats.Instance.SurviveTime;
                else
                    return 0f;
            default:
                return 0f;
        }
    }

    void UnlockAchievement(int AchievementIndex)
    {
        if (!AchievementGranted.Contains(AchievementIndex))
        {
            AchievementGranted.Add(AchievementIndex);
            Debug.Log("成就達成: 第 " + AchievementIndex + " 項");
        }
    }
}
