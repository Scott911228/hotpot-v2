using UnityEngine;

[System.Serializable]
public class AchievementSet
{
    public enum AchievementTypeEnum
    {
        None,
        WinLevel,
        BaseHealth,
        KillCount,
        DispatchSpecificCount,
        DispatchTotalCount,
        SurviveTime
    }
    public enum AchievementValueJudgeEnum
    {
        None,
        Above,
        Equal,
        Below
    }
    [Header("達成條件種類")]
    public AchievementTypeEnum AchievementType; // 使用Enum
    [Header("判定方式")]
    public AchievementValueJudgeEnum AchievementValueJudge; // 使用Enum
    [Header("數值")]
    public float value;
    [Header("指定派遣角色")]
    public GameObject character; // 使用DispatchSpecificCount才有效
}