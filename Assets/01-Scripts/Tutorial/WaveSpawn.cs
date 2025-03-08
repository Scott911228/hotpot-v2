using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class WaveSpawn : MonoBehaviour
{
    public static int EnemiesAlive = 0;
    public GameManager gameManager;
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
    public PathsManager pathsManager; // 路徑管理器

    void Start()
    {
        EnemyWaves = GameObject.Find("LevelSettings").GetComponent<LevelSettings>().EnemyWaves;
        EnemiesAlive = 0;
        waveNumber = 0;
        InvokeRepeating("UpdateEnemyCountText", 0f, 0.1f);
        pathsManager = GameObject.Find("PathsManager").GetComponent<PathsManager>(); // 取得 PathsManager
        SpawnEnemy();
    }

    void Update()
    {
        if (TotalEnemyCount - KilledEnemyCount > 0)
        {
            return;
        }
        if (!GameObject.Find("GameControl").GetComponent<GameManager>().isGamePlaying)
        {
            return;
        }

        if (waveNumber == EnemyWaves.Length)
        {
            gameManager.WinLevel();
            this.enabled = false;
        }

        if (countdown <= 0f)
        {
            StartCoroutine(SpawnWave());
            countdown = timeBetweenWaves;
            return;
        }

        countdown -= Time.deltaTime;
        countdown = Mathf.Clamp(countdown, 0f, Mathf.Infinity);
        waveCountdownText.text = "第 " + (waveNumber + 1).ToString() + " 波 ... " + string.Format("{0:00.00}", countdown);
    }

    public void UpdateEnemyCountText()
    {
        string EnemyCountString = (TotalEnemyCount - KilledEnemyCount).ToString();
        if (EnemyCountString != EnemyCountText.text)
        {
            EnemyCountText.text = EnemyCountString;
            Animator = EnemyCountText.GetComponent<Animator>();
            Animator.enabled = true;
            Animator.Play("Start", 0, 0f);
        }
        if (waveNumber > 0) WaveCountText.text = "第 " + waveNumber.ToString() + " / " + EnemyWaves.Length.ToString() + " 波";
    }

    IEnumerator SpawnWave()
    {
        Wave wave = EnemyWaves[waveNumber];
        EnemyContent[] enemyContent = wave.Enemy;
        GameObject.Find("GameControl").GetComponent<PlayerStats>().Rounds++;

        // ===== 計算敵人總數 =====
        foreach (var e in enemyContent)
        {
            TotalEnemyCount += e.spawnCount;
        }
        waveNumber++;

        // ===== 開始生成敵人 =====
        for (int i = 0; i < enemyContent.Length; i++)
        {
            EnemyContent e = enemyContent[i];

            // 取得對應的出生點和路徑
            if (i < pathsManager.GetPathCount())
            {
                PathsManager.PathData pathData = pathsManager.GetPathData(i); // 每個spawn point有自己獨立的路徑
                Transform spawnPoint = pathData.spawnPoint;

                for (int j = 0; j < e.spawnCount; j++)
                {
                    while (!GameObject.Find("GameControl").GetComponent<GameManager>().isGamePlaying)
                    {
                        yield return new WaitForSeconds(0.1f);
                    }
                    SpawnEnemy(e, spawnPoint, pathData.path); // 把spawn point和path傳遞給SpawnEnemy
                    yield return new WaitForSeconds(1f / e.spawnRate);
                }
            }

            yield return new WaitForSeconds(e.delayToNextContent);
        }
    }

    void SpawnEnemy()
    {
        if (pathsManager != null)
        {
            // 根據路徑資料生成敵人並初始化路徑
            for (int i = 0; i < pathsManager.GetPathCount(); i++)
            {
                PathsManager.PathData pathData = pathsManager.GetPathData(i);
                GameObject enemy = Instantiate(enemyPrefab, pathData.spawnPoint.position, Quaternion.identity);
                
                // 假設 Enemies 類別內有移動邏輯，可以直接初始化
                Enemies enemyScript = enemy.GetComponent<Enemies>();
                enemyScript.InitializePath(pathData.path);  // 設定敵人的路徑
            }
        }
        // 設置敵人的路徑
        enemy.GetComponent<EnemyMovement>().SetPath(path); // 假設有一個EnemyMovement腳本負責移動，並設置路徑

        // ===== 血量加成 =====
        if (enemyContent.healthMultiplier != 0)
        {
            enemyStatus.StartHealth = (int)(enemyStatus.StartHealth * enemyContent.healthMultiplier);
            enemyStatus.Health = enemyStatus.StartHealth;
        }

        // ===== 傷害加成 =====
        if (enemyContent.damageMultiplier != 0)
        {
            GameObject bullet = enemyAttackScript.bulletPrefab;
            EnemyBullets enemyBullets = bullet.GetComponent<EnemyBullets>();
            enemyBullets.damageMultiplier = enemyContent.damageMultiplier;
        }

        // ===== 速度加成 =====
        if (enemyContent.speedMultiplier != 0)
        {
            enemyStatus.speedMultiplier = enemyContent.speedMultiplier;
        }

        EnemiesAlive++;
        spawnedCount++;
    }
}
