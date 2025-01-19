using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private float _timeToDrain = 0.25f;
    [SerializeField] private Gradient _healthBarGradient;
    private Image _image;
    private float _target = 1f;
    private Coroutine drainHealthBarCoroutine;
    private Color _newHealthBarColor;
    private void Start()
    {
        _image = GetComponent<Image>();
        _image.color = _healthBarGradient.Evaluate(_target);
        CheckHealthBarGradientAmount();
    }
    public void UpdateHealthBar(float maxHealth, float currentHealth)
    {
        _target = currentHealth / maxHealth;
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
