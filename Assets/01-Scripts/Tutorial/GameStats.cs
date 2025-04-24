using UnityEngine;
using System.Collections.Generic;
using System;

public class GameStats : MonoBehaviour
{
    public static GameStats Instance;
    public bool LevelCleared;
    public int EnemyKilledCount = 0;
    public int DeployedCharacterCount = 0;
    // 使用 Dictionary 來追蹤特定角色的派遣數量
    public Dictionary<GameObject, int> characterDispatchCount = new Dictionary<GameObject, int>();


    public int TargetHP => LevelSettings.Instance != null ? LevelSettings.Instance.Life : 0;

    public float SurviveTime { get; private set; } //限時模式

    void Start()
    {
        GameStats.Instance.ResetCharacterDispatchCount();
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
        }
    }

    private void Update()
{
    if (!LevelCleared)
    {
        SurviveTime += Time.deltaTime;
    }
}

    public void ResetCharacterDispatchCount()
    {
        characterDispatchCount.Clear();
        DeployedCharacterCount = 0;
        Debug.Log("所有角色的派遣次數已重置。");
    }
    public void RegisterCharacterDispatch(GameObject character)
    {
        if (character == null)
        {
            Debug.LogError("RegisterCharacterDispatch(): character 為 NULL！");
            return;
        }

        if (!characterDispatchCount.ContainsKey(character))
        {
            characterDispatchCount[character] = 0;
        }

        characterDispatchCount[character]++;
        DeployedCharacterCount++;
        Debug.Log($"角色 {character.name} 已派遣 {characterDispatchCount[character]} 次");
    }

    // 取得特定角色的派遣次數
    public int GetCharacterDispatchCount(GameObject character)
    {
        if (character == null)
        {
            Debug.LogError("GetCharacterDispatchCount(): character 為 NULL！");
            return 0;
        }

        if (characterDispatchCount.TryGetValue(character, out int count))
        {
            return count;
        }

        Debug.LogWarning($"角色 {character.name} 尚未派遣過。");
        return 0;
    }
}
