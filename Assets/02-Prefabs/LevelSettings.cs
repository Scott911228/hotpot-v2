using UnityEngine;

public class LevelSettings : MonoBehaviour
{
    [Header("關卡敘述")]
    // ======= 關卡名稱 ======= 
    public string StageName = "";
    [Header("開場數值")]
    // ======= 金錢 ======= 
    public double Money = 100;
    // ======= 基地生命 ======= 
    public int Life = 1;
    // ======= 角色內容 ======= 
    [Header("角色內容")]
    public CharacterSet[] characterSets;

    // ======= 關卡敵人內容 =======
    [Header("關卡敵人內容")]
    public Wave[] EnemyWaves;
    // ======= 湯底效果 ======= 
    [Header("湯底效果")]
    public CharacterDispatchLimit[] characterDispatchLimits;
    // ======= 湯底效果 ======= 
    [Header("湯底效果")]
    public string FlavorName;
    public string FlavorDescription;
    public Color FlavorColor = Color.white;
    public float CharacterDamageMultiplier = 1.0f;
    public float CharacterAttackSpeedMultiplier = 1.0f;
    public float CharacterHealthMultiplier = 1.0f;
    public float EnemyDamageMultiplier = 1.0f;
    public float EnemyAttackSpeedMultiplier = 1.0f;
    public float EnemyHealthMultiplier = 1.0f;
    public float EnemyMovementMultiplier = 1.0f;
}
