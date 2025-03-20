using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEditor.SearchService;
using UnityEngine.SceneManagement;

public class WinAnimControl : MonoBehaviour
{
    public TextMeshProUGUI levelClearText;
    public GameObject achivment;
    public string loadSceneName;

    void Start()
    {
        PlayWinAnim();
    }

    void PlayWinAnim()
    {
        levelClearText.transform.localScale = Vector3.zero;
        achivment.transform.localScale = Vector3.zero;
        levelClearText.transform.DOScale(1, 1.0f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            achivment.transform.DOScale(1, 1.0f).SetEase(Ease.OutExpo).OnComplete(() =>
            {
                DOVirtual.DelayedCall(2.0f, () =>
                {
                    SceneManager.LoadScene(loadSceneName);
                });
            });
        });
    }


}

