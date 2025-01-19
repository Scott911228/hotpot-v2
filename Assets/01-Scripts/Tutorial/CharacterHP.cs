using UnityEngine;
using UnityEngine.UI;
using System;

public class CharacterHP : MonoBehaviour
{
    public HealthBar HealthBar; // 健康條的參考
    Animator myAnimator; // 動畫控制器
    public int StartHealth = 200; // 初始生命值
    private float Health; // 當前生命值
    public bool isDead = false; // 是否死亡
    private static bool hasEverDied = false; // 紀錄是否已經死亡過
    private bool isProtected = false;
    private float DamageMultiplier;
    private float protectTime;
    void Start()
    {
        myAnimator = GetComponent<Animator>();
        Health = StartHealth; // 初始化生命值
        HealthBar.UpdateHealthBar(StartHealth, Health); // 更新健康條
        InvokeRepeating("ProtectEffectCheck", 0f, 0.5f);
    }
    public void HealToFull()
    {
        Health = StartHealth;
        HealthBar.UpdateHealthBar(StartHealth, Health);
    }
    void ProtectEffectCheck() // 
    {
        if (isProtected)
        {
            if (protectTime > 0.5f)
            {
                protectTime -= 0.5f;
            }
            else
            {
                isProtected = false;
            }
        }
    }
    public void AddProtect(float _DamageMultiplier, float effectLength)
    {
        isProtected = true;
        DamageMultiplier = _DamageMultiplier;
        protectTime = effectLength;
    }
    // 接收傷害
    public void TakeDamage(float amount)
    {
        if (isProtected)
        {
            amount *= DamageMultiplier;
            amount = Convert.ToInt32(amount);
        }
        Health -= amount; // 減少生命值
        GameObject.Find("GameControl").GetComponent<GameManager>().DisplayDamage(
            gameObject,
            (isProtected ? "減傷! " : "") + amount.ToString(),
            isProtected ? 30 : 40,
            new Color(0.09411757f, 0.05882348f, 0.05882348f, 0.8705882f));
        Health = Mathf.Max(Health, 0); // 確保不低於 0
        HealthBar.UpdateHealthBar(StartHealth, Health); // 更新健康條

        if (Health <= 0 && !isDead)
        {
            Die(); // 如果死亡則呼叫死亡函式
        }
    }
    // 接收治療
    public void ReceiveHealing(float amount)
    {
        Health += amount;
        GameObject.Find("GameControl").GetComponent<GameManager>().DisplayDamage(
            gameObject,
            amount.ToString(),
            40,
            new Color(0.5228668f, 0.7924528f, 0.4597722f, 0.7019608f));
        Health = Mathf.Min(Health, StartHealth); // 確保不超過最大 HP
        HealthBar.UpdateHealthBar(StartHealth, Health);
        Debug.Log("Received healing: " + amount + ", current HP: " + Health);
    }


    // 處理死亡邏輯
    void Die()
    {
        if (!hasEverDied)
        {
            TextControl.BroadcastControlMessage("tutorial/text3");// 顯示死亡教學文本
            hasEverDied = true; // 設置為已經死亡
        }
        isDead = true; // 設置死亡狀態
        Destroy(gameObject); // 刪除角色物件
    }

    // 獲取當前生命值
    public float GetCurrentHP()
    {
        return Health; // 返回當前 HP
    }
}
