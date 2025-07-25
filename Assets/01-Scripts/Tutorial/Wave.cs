﻿using UnityEngine;

[System.Serializable]
public class Wave
{
    public EnemyContent[] Enemy;
    public bool isInfinitySpawn;
}

[System.Serializable]
public class EnemyContent
{
    [Header("生成內容")]
    public GameObject Enemy;
    [Header("路徑編號")]
    public int pathIndex;
    [Header("生成頻率設定")]
    public int spawnCount;
    public float spawnRate;
    public float delayToNextContent;
    [Header("敵人尺寸")]
    public float minScale;
    public float maxScale;
    [Header("敵人屬性加成")]
    public float damageMultiplier;
    public float healthMultiplier;
    public float speedMultiplier;
}
