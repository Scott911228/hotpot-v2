using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.IO;

public class WaveSpawn : MonoBehaviour
{
    public static int EnemiesAlive = 0;
    public GameManager gameManager;
    private Transform Target;
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

    void Start()
    {
        pathSettings = GameObject.Find("PathSettings")?.GetComponent<PathSettings>(); // 取得 PathSettings
        EnemyWaves = GameObject.Find("LevelSettings").GetComponent<LevelSettings>().EnemyWaves;
        EnemiesAlive = 0;
        waveNumber = 0;
        InvokeRepeating("UpdateEnemyCountText", 0f, 0.1f);
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
            StartCoroutine(SpawnWaveEnemies());  // 使用協程生成敵人
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

    // 協程控制每波敵人生成
    IEnumerator SpawnWaveEnemies()
    {

        if (GameObject.Find("LevelSettings").GetComponent<LevelSettings>().StageName == "第二關")
        {
            if (waveNumber == 1)
            {
                TextControl.BroadcastControlMessage("tutorial2/text3");
            }
            if (waveNumber == 2)
            {
                TextControl.BroadcastControlMessage("tutorial2/text5");
            }
        }
        //KilledEnemyCount = 0;
        Wave currentWave = EnemyWaves[waveNumber];
        TotalEnemyCount = 0;

        foreach (EnemyContent content in currentWave.Enemy)
        {
            TotalEnemyCount += content.spawnCount;

            for (int i = 0; i < content.spawnCount; i++)
            {
                SpawnEnemy(content);  // 生成敵人
                yield return new WaitForSeconds(content.spawnRate);  // 控制敵人生成的頻率
            }

            yield return new WaitForSeconds(content.delayToNextContent);  // 延遲下一個敵人生成
        }

        waveNumber++;
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
            
            GameStats.Instance.AddEnemy();
            EnemiesAlive++;
        }
    }

}
