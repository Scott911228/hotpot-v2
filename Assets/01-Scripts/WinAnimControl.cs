using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class WinAnimControl : MonoBehaviour
{
    public Image levelClearText;
    public string loadSceneName;
    public GameObject[] achievements;
    public CanvasGroup achievementCanvasGroup;

    void Start()
    {

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

    void ShowCanvasGroup()
    {
        achievementCanvasGroup.DOFade(1, 0.5f);
    }
}

