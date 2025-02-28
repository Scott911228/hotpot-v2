using UnityEngine;

[System.Serializable]
public class Wave
{
    public EnemyContent[] Enemy;
}

[System.Serializable]
public class EnemyContent
{
    [Header("生成內容")]
    public GameObject Enemy;
    [Header("生成頻率設定")]
    public int spawnCount;
    public float spawnRate;
    public float delayToNextContent;
    [Header("敵人屬性加成")]
    public float damageMultiplier;
    public float healthMultiplier;
    public float speedMultiplier;
}
