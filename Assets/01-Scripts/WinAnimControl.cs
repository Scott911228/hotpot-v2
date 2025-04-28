using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class WinAnimControl : MonoBehaviour
{
    public Image levelClearText;
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
        levelClearText.transform.DOScale(15, 1.0f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            DOVirtual.DelayedCall(2.0f, () =>
            {
                SceneManager.LoadScene(loadSceneName);
            });
        });
    }


}

