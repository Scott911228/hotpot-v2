using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEditor.SearchService;
using UnityEngine.SceneManagement;
public class LoseAnimControl : MonoBehaviour
{
    public Image gameOverText;
    public Image encourageText;
    public CanvasGroup Buttons;
    public string loadSceneName;

    void Start()
    {
        PlayLoseAnim();
    }

    void PlayLoseAnim()
    {

        gameOverText.DOFade(1, 1.0f).OnComplete(() =>
        {
            DOVirtual.DelayedCall(1.0f, () =>
                {
                    gameOverText.transform.DOLocalMove(new Vector3(0, 250, 0), 1).SetEase(Ease.InOutSine).OnComplete(() =>
                    {
                        ShowCanvasGroup();
                    });
                });

        });
    }

    void ShowCanvasGroup()
    {
        encourageText.DOFade(1, 0.5f);
        Buttons.DOFade(1, 1.0f);
    }

}

