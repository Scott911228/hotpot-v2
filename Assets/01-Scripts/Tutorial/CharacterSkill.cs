using UnityEngine;

public class CharacterSkill : MonoBehaviour
{
    public float targetMP = 100;
    public float currentMP = 0;
    public float gainMPspeed = 0.1f;
    public HealthBar HealthBar;
    void Start()
    {
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
            "Skill OK!",
            40,
            new Color(0.09411757f, 0.05882348f, 0.05882348f, 0.8705882f));
        }
    }
}
