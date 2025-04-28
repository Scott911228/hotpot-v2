using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class WaveSpawn : MonoBehaviour
{
    private WaveBroadcastMessage[] broadcastMessages; // Inspector 設定
    private HashSet<string> playedMessages = new HashSet<string>();

    private string CurrentStageName => GameObject.Find("LevelSettings")?.GetComponent<LevelSettings>()?.StageName;

    public static int EnemiesAlive = 0;
    public GameManager gameManager;
    private Transform Target;
    private bool isSummoning;
    public float timeBetweenWaves = 5f;
    public float countdown = 5f;
    public static int KilledEnemyCount = 0;
    public GameObject BroadcastMessagePanel;
    public Animator BroadcastMessageAnimator;
    public Animator Animator;
    private Wave[] EnemyWaves;
    private int spawnedCount = 0;
    private int TotalEnemyCount = 0;

    [Header("敵人總數文字")]
    public string enemyTag = "Enemy";
    public string prefix = "";
    public string suffix = "";
    public Text EnemyCountText;
    public Text WaveCountText;

    [Header("下一波倒數")]
    public Text waveCountdownText;
    public static int waveNumber = 0;

    [Header("路徑管理")]
    public PathSettings pathSettings; // 路徑管理器
    public GameObject enemyPrefab; // 敵人預製物
    private GameObject[] remainingEnemies;

    private float _surviveTimeTarget;
    private float surviveTimeElapsed = 0f;
    private bool _isTimeLevelType = false;
    void Start()
    {
        pathSettings = GameObject.Find("PathSettings")?.GetComponent<PathSettings>(); // 取得 PathSettings
        EnemyWaves = GameObject.Find("LevelSettings").GetComponent<LevelSettings>().EnemyWaves;
        broadcastMessages = GameObject.Find("LevelSettings").GetComponent<LevelSettings>().broadcastMessages;
        _isTimeLevelType = GameObject.Find("LevelSettings").GetComponent<LevelSettings>().LevelType == LevelType.LevelTypeEnum.Time;
        _surviveTimeTarget = GameObject.Find("LevelSettings").GetComponent<LevelSettings>().surviveTimeTarget;
        EnemiesAlive = 0;
        waveNumber = 0;
        InvokeRepeating("UpdateEnemyCountText", 0f, 0.1f);
    }

    private bool IsInfinityWaveActive()
    {
        if (waveNumber >= EnemyWaves.Length) return false;
        return EnemyWaves[waveNumber].isInfinitySpawn;
    }

    void Update()
    {
        if(!GameObject.Find("GameControl").GetComponent<GameManager>().isGamePlaying) return;
        if (_isTimeLevelType)
        {
            if (TotalEnemyCount > 0)
            {
                surviveTimeElapsed += Time.deltaTime;
                //Debug.Log($"{surviveTimeElapsed} / {_surviveTimeTarget}"); // 顯示已經過的秒數與達成條件
                if (surviveTimeElapsed >= _surviveTimeTarget)
                {
                    gameManager.WinLevel();
                    enabled = false;
                    return;
                }
            }
        }
        if (isSummoning) // 如果正在生成敵人，就不偵測是否重新開始生成
        {
            return;
        }
        if (!GameObject.Find("GameControl").GetComponent<GameManager>().isGamePlaying) // 如果正在暫停，就不偵測是否重新開始生成
        {
            return;
        }

        if (remainingEnemies?.Length > 0) // 如果場上還有敵人，就不偵測是否重新開始生成
        {
            return;
        }
        if (waveNumber >= EnemyWaves.Length)
        {
            if (!IsInfinityWaveActive()) // 若不是無限波次才直接判勝
            {
                gameManager.WinLevel();
                enabled = false;
                return;
            }
        }
        if (countdown <= 0f)
        {
            StartCoroutine(SpawnWaveEnemies());  // 使用協程生成敵人
            countdown = timeBetweenWaves;
            return;
        }
        countdown -= Time.deltaTime;
        countdown = Mathf.Clamp(countdown, 0f, Mathf.Infinity);
        waveCountdownText.text = "敵人即將出現... " + string.Format("{0:00.00}", countdown);
    }

    public void UpdateEnemyCountText()
    {
        if (_isTimeLevelType)
        {
            EnemyCountText.text = (_surviveTimeTarget - surviveTimeElapsed).ToString("F1");
        }
        else
        {
            remainingEnemies = GameObject.FindGameObjectsWithTag("Enemy");
            string EnemyCountString = remainingEnemies.Length.ToString();
            if (EnemyCountString != EnemyCountText.text) // 更新敵人數量顯示
            {
                EnemyCountText.text = EnemyCountString;
                Animator = EnemyCountText.GetComponent<Animator>();
                Animator.enabled = true;
                Animator.Play("Start", 0, 0f);
            }
            if (isSummoning)
            {
                return;
            }
            if (waveNumber > 0) WaveCountText.text = "第 " + waveNumber.ToString() + " / " + EnemyWaves.Length.ToString() + " 波";
        }
    }
    private void TriggerWaveBroadcastMessage()
    {
        string currentStage = CurrentStageName;
        if (string.IsNullOrEmpty(currentStage)) return;

        foreach (var msg in broadcastMessages)
        {
            if (msg.stageName == currentStage && msg.waveNumber == waveNumber && !playedMessages.Contains(msg.messageKey))
            {
                TextControl.BroadcastControlMessage(msg.messageKey);
                playedMessages.Add(msg.messageKey);
                break;
            }
        }
    }
    // 協程控制每波敵人生成
    IEnumerator SpawnWaveEnemies()
    {
        WaveCountText.text = "第 " + (waveNumber + 1).ToString() + " / " + EnemyWaves.Length.ToString() + " 波";
        TriggerWaveBroadcastMessage();

        if (waveNumber >= EnemyWaves.Length) yield break;

        Wave currentWave = EnemyWaves[waveNumber];
        TotalEnemyCount = 0;
        isSummoning = true;

        do
        {
            foreach (EnemyContent content in currentWave.Enemy)
            {
                TotalEnemyCount += content.spawnCount;

                for (int i = 0; i < content.spawnCount; i++)
                {
                    while (!GameObject.Find("GameControl").GetComponent<GameManager>().isGamePlaying)
                        yield return null;

                    SpawnEnemy(content);
                    yield return new WaitForSeconds(content.spawnRate);
                }

                yield return new WaitForSeconds(content.delayToNextContent);
            }

            // 如果是無限波次，就不停重複，不增加 waveNumber
            if (!currentWave.isInfinitySpawn)
            {
                waveNumber++;
                break;
            }

            yield return new WaitForSeconds(timeBetweenWaves); // 無限波次之間也稍微等待
        }
        while (true);

        isSummoning = false;
        countdown = timeBetweenWaves;
    }


    // 生成敵人的邏輯
    void SpawnEnemy(EnemyContent content)
    {
        {
            PathSettings.PathData pathData = pathSettings.GetPathData(content.pathIndex);
            if (pathData == null)
            {
                Debug.LogError($"找不到路徑索引 {content.pathIndex}，請檢查 PathSettings 設置！");
                return;
            }

            GameObject enemy = Instantiate(content.Enemy, pathData.spawnPoint.position, Quaternion.Euler(0, -90, 0));
            Enemies enemyScript = enemy.GetComponent<Enemies>();

            // 確保敵人使用自己的路徑
            Transform[] points = pathSettings.GetPoints(content.pathIndex);
            if (points == null || points.Length == 0)
            {
                Debug.LogError($"敵人 {enemy.name} 的路徑為空，請檢查 PathSettings 是否設置正確！");
                return;
            }

            enemyScript.InitializePath(points);

            // 設定速度、血量、傷害
            if (content.healthMultiplier != 0)
            {
                enemyScript.StartHealth = (int)(enemyScript.StartHealth * content.healthMultiplier);
                enemyScript.Health = enemyScript.StartHealth;
            }
            if (content.speedMultiplier != 0)
            {
                enemyScript.speedMultiplier = content.speedMultiplier;
            }
            EnemiesAlive++;
        }
    }

}
