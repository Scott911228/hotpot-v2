using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private float _timeToDrain = 0.25f;
    [SerializeField] private Gradient _healthBarGradient;
    private Image _image;
    public int barType;
    private float _target = 1f;
    private CanvasGroup parentCanvasGroup;
    private Coroutine drainHealthBarCoroutine;
    private Color _newHealthBarColor;
    private void Start()
    {
        // 取得父物件
        Transform parent = transform.parent;

        if (parent != null)
        {
            // 查找父物件上的 CanvasGroup
            parentCanvasGroup = parent.GetComponent<CanvasGroup>();

            if (parentCanvasGroup != null)
            {
                Debug.Log("父物件的 CanvasGroup：" + parentCanvasGroup.name);
            }
            else
            {
                Debug.LogWarning("父物件未找到 CanvasGroup");
            }
        }
        else
        {
            Debug.LogWarning("未找到父物件");
        }
        _image = GetComponent<Image>();
        _image.color = _healthBarGradient.Evaluate(_target);
        CheckHealthBarGradientAmount();
    }
    public void UpdateHealthBar(float maxHealth, float currentHealth)
    {
        if (parentCanvasGroup == null)
        {
            Transform parent = transform.parent;
            // 查找父物件上的 CanvasGroup
            parentCanvasGroup = parent.GetComponent<CanvasGroup>();

            if (parentCanvasGroup != null)
            {
                Debug.Log("父物件的 CanvasGroup：" + parentCanvasGroup.name);
            }
            else
            {
                Debug.LogWarning("父物件未找到 CanvasGroup");
            }
        }
        else
        {
            Debug.LogWarning("未找到父物件");
        }
        _target = currentHealth / maxHealth;
        if (_target >= 1)
        {
            GetComponent<CanvasGroup>().alpha = 0f;
            parentCanvasGroup.alpha = 0f;
        }
        else
        {
            GetComponent<CanvasGroup>().alpha = 1f;
            parentCanvasGroup.alpha = 1f;
        }
        drainHealthBarCoroutine = StartCoroutine(DrainHealthBar());
        CheckHealthBarGradientAmount();
    }
    private IEnumerator DrainHealthBar()
    {
        _image = GetComponent<Image>();
        float fillAmount = _image.fillAmount;
        Color currentColor = _image.color;
        float elapsedTime = 0f;
        while (elapsedTime < _timeToDrain)
        {
            elapsedTime += Time.deltaTime;
            _image.fillAmount = Mathf.Lerp(fillAmount, _target, (elapsedTime / _timeToDrain));
            _image.color = Color.Lerp(currentColor, _newHealthBarColor, (elapsedTime / _timeToDrain));
            yield return null;
        }
    }
    private void CheckHealthBarGradientAmount()
    {
        _newHealthBarColor = _healthBarGradient.Evaluate(_target);
    }
}
