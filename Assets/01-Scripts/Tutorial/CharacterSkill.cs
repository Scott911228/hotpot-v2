using UnityEngine;
using System.Collections;

public class CharacterSkill : MonoBehaviour
{
    public float targetMP = 100;
    public float currentMP = 0;
    public float gainMPspeed = 0.1f;
    public string skillName;
    [TextArea]
    public string skillDesc;
    [TextArea]
    public string skillOtherDetail;

    public HealthBar HealthBar;
    private Animator animator;
    private float originalMultiplier;
    private Coroutine currentBoostCoroutine;
    void Start()
    {
        animator = GetComponent<Animator>();
        originalMultiplier = animator.GetFloat("runMultiplier");
        currentMP = 0; // 初始化 MP
        HealthBar.UpdateHealthBar(targetMP, currentMP); // 更新 MP 條
        InvokeRepeating("IncreaseMP", 0f, 0.1f);
    }
    void IncreaseMP()
    {
        if (!GameObject.Find("GameControl").GetComponent<GameManager>().isGamePlaying) return;
        if (currentMP >= targetMP) return;
        currentMP += gainMPspeed;
        HealthBar.UpdateHealthBar(targetMP, currentMP);
        if (currentMP >= targetMP)
        {
            GameObject.Find("GameControl").GetComponent<GameManager>().DisplayFloatingText(
            gameObject,
            "技能 OK!",
            40,
            new Color(0.09411757f, 0.05882348f, 0.05882348f, 0.8705882f));
        }
    }
    public void RunSkill()
    {
        currentMP = 0;
        HealthBar.UpdateHealthBar(targetMP, currentMP);
        BoostRunMultiplier(2f, 10f);
        GameObject.Find("GameControl").GetComponent<GameManager>().DisplayFloatingText(
        gameObject,
        "發動技能 !",
        40,
        new Color(0.09411757f, 0.05882348f, 0.05882348f, 0.8705882f));
    }

    public void BoostRunMultiplier(float multiplier = 1.5f, float duration = 3f)
    {
        if (currentBoostCoroutine != null)
            StopCoroutine(currentBoostCoroutine);

        currentBoostCoroutine = StartCoroutine(RunMultiplierBoostRoutine(multiplier, duration));
    }
    private IEnumerator RunMultiplierBoostRoutine(float targetMultiplier, float duration)
    {
        float elapsed = 0f;
        float startMultiplier = originalMultiplier;

        // 升速階段：從原值 → 目標值
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float newMultiplier = Mathf.Lerp(startMultiplier, startMultiplier * targetMultiplier, t);
            animator.SetFloat("runMultiplier", newMultiplier);
            yield return null;
        }

        // 回復階段（你可以選擇立即回復或平滑回復）
        float returnDuration = 0.5f;
        float returnElapsed = 0f;
        float boosted = animator.GetFloat("runMultiplier");

        while (returnElapsed < returnDuration)
        {
            returnElapsed += Time.deltaTime;
            float t = returnElapsed / returnDuration;
            float newMultiplier = Mathf.Lerp(boosted, originalMultiplier, t);
            animator.SetFloat("runMultiplier", newMultiplier);
            yield return null;
        }

        animator.SetFloat("runMultiplier", originalMultiplier);
        currentBoostCoroutine = null;
    }
}
