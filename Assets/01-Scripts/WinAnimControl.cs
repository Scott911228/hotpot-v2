using UnityEngine;
using TMPro;
using DG.Tweening;

public class PopUpText : MonoBehaviour
{
    public TextMeshProUGUI levelClearText;

    void Start()
    {
        levelClearText.transform.localScale = Vector3.zero;
        levelClearText.transform.DOScale(1, 1.0f).SetEase(Ease.OutBack);
    }
}

