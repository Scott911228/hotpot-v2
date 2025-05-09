using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class WinAnimControl : MonoBehaviour
{
    public Image levelClearText;
    public GameObject[] achievements;
    public CanvasGroup achievementCanvasGroup;
    public string loadSceneName;
    private AchievementSet[] AchievementSets;
    public Sprite StarLightedSprite;
    public Sprite StarNotLightedSprite;

    void Start()
    {
        LevelSettings levelSettings = GameObject.Find("LevelSettings")?.GetComponent<LevelSettings>();
        if (levelSettings != null)
        {
            AchievementSets = levelSettings.AchievementSets;
        }
        else
        {
            Debug.LogWarning("LevelSettings not found or missing AchievementSets.");
        }

        PlayWinAnim();
    }

    void PlayWinAnim()
    {
        levelClearText.transform.localScale = Vector3.zero;
        levelClearText.transform.DOScale(15, 1.0f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            DOVirtual.DelayedCall(1.0f, () =>
            {
                levelClearText.transform.DOLocalMove(new Vector3(0, 150, 0), 1).SetEase(Ease.InOutSine).OnComplete(() =>
                        {
                            ShowCanvasGroup();
                            DOVirtual.DelayedCall(3.0f, () =>
                            {
                                SceneManager.LoadScene(loadSceneName);
                            });
                        }
                        );
            });
        });
    }
    void AutoFillAchievementTextAndSprite()
    {
        int achievementCount = Mathf.Min(achievements.Length, AchievementSets.Length);
        HashSet<int> grantedAchievements = AchievementManager.Instance.CheckGrantedAchievement();
        for (int i = 0; i < achievementCount; i++)
        {
            GameObject achievement = achievements[i];
            Text text = achievement.GetComponentInChildren<Text>();
            Image image = achievement.GetComponentInChildren<Image>();
            if (text == null)
            {
                Debug.LogWarning($"Achievement {achievement.name} is missing a Text component.");
                continue;
            }
            string baseText;
            switch (AchievementSets[i].AchievementType)
            {
                case AchievementSet.AchievementTypeEnum.None:
                    baseText = "無";
                    break;
                case AchievementSet.AchievementTypeEnum.WinLevel:
                    baseText = "完成關卡";
                    break;
                case AchievementSet.AchievementTypeEnum.BaseHealth:
                    baseText = "我方基地生命剩餘{judge} {value}";
                    break;
                case AchievementSet.AchievementTypeEnum.KillCount:
                    baseText = "擊殺{judge} {value} 個敵人";
                    break;
                case AchievementSet.AchievementTypeEnum.DispatchTotalCount:
                    baseText = "合計{judge}部屬 {value} 次角色";
                    break;
                case AchievementSet.AchievementTypeEnum.SurviveTime:
                    baseText = "存活{judge} {value} 秒";
                    break;
                case AchievementSet.AchievementTypeEnum.DispatchSpecificCount:
                    baseText = "合計{judge}部屬 {value} 次{name}";
                    break;
                default:
                    baseText = "無";
                    break;
            }

            switch (AchievementSets[i].AchievementValueJudge)
            {
                case AchievementSet.AchievementValueJudgeEnum.None:
                    baseText = "無";
                    break;
                case AchievementSet.AchievementValueJudgeEnum.Above:
                    baseText = baseText.Replace("{judge}", "至少");
                    break;
                case AchievementSet.AchievementValueJudgeEnum.Equal:
                    baseText = baseText.Replace("{judge}", "剛好");
                    break;
                case AchievementSet.AchievementValueJudgeEnum.Below:
                    baseText = baseText.Replace("{judge}", "未滿");
                    break;
                default:
                    baseText = "無";
                    break;
            }

            baseText = baseText.Replace("{value}", AchievementSets[i].value.ToString());
            if (AchievementSets[i].AchievementType == AchievementSet.AchievementTypeEnum.DispatchSpecificCount)
            {
                if (AchievementSets[i].character != null)
                {
                    Character character = AchievementSets[i].character.GetComponent<Character>();
                    if (character != null)
                    {
                        baseText = baseText.Replace("{name}", character.characterName);
                    }
                    else
                    {
                        Debug.LogWarning($"Character in AchievementSet {i} is missing a Character component.");
                    }
                }
            }
            text.text = baseText;
            if (grantedAchievements.Contains(i))
            {
                image.sprite = StarLightedSprite;
            }
            else
            {
                image.sprite = StarNotLightedSprite;
            }
        }
    }


    void ShowCanvasGroup()
    {
        AutoFillAchievementTextAndSprite();
        achievementCanvasGroup.DOFade(1, 0.5f);
    }
}

