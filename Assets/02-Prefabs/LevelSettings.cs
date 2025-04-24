using UnityEngine;

public class LevelSettings : MonoBehaviour
{
    public static LevelSettings Instance;

    [Header("關卡敘述")]
    // ======= 關卡名稱 ======= 
    public string StageName = "";
    // ======= 關卡類型 ======= 
    public LevelType.LevelTypeEnum LevelType; // 使用Enum
    [Header("開場數值")]
    // ======= 金錢 ======= 
    public double Money = 100;
    // ======= 基地生命 ======= 
    public int Life;
    // ======= 時間限制 ======= 
     public float TimeLimit = 60f;
    // ======= 角色內容 ======= 
    [Header("角色內容")]
    public CharacterSet[] characterSets;
    // ======= 關卡敵人內容 =======
    [Header("關卡敵人內容")]
    public Wave[] EnemyWaves;
    // ======= 關卡成就 ======= 
    [Header("關卡成就（最多 3 個）")]
    public AchievementSet[] AchievementSets;
    void OnValidate()
    {
        if (AchievementSets.Length > 3)
        {
            Debug.LogWarning("AchievementSets 的長度無法超過 3 個！");
            System.Array.Resize(ref AchievementSets, 3);
        }
    }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    // ======= 角色派遣上限 ======= 
    [Header("角色派遣上限")]
    public CharacterDispatchLimit[] characterDispatchLimits;
    // ======= 劇情波次綁定 ======= 
    [Header("劇情波次綁定（對接Fungus）")]
    public WaveBroadcastMessage[] broadcastMessages;


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
