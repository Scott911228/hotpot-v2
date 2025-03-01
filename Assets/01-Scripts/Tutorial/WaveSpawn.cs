using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using System;
public class WaveSpawn : MonoBehaviour
{
    public static int EnemiesAlive = 0;
    public Transform enemyPrefab;
    public Transform spawnPoint;
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

    void Start()
    {
        EnemyWaves = GameObject.Find("LevelSettings").GetComponent<LevelSettings>().EnemyWaves;
        EnemiesAlive = 0;
        waveNumber = 0;
        InvokeRepeating("UpdateEnemyCountText", 0f, 0.1f);
    }

    public void Update()
    {
        if (TotalEnemyCount - KilledEnemyCount > 0)
        {
            return;
        }
        if (!GameObject.Find("GameControl").GetComponent<GameManager>().isGamePlaying)
        {
            return;
        }

        //if(!BroadcastMessagePanel.activeSelf && waveNumber > 0) BroadcastMessagePanel.SetActive(true);
        if (waveNumber == EnemyWaves.Length)
        {
            gameManager.WinLevel();
            this.enabled = false;
        }

        if (countdown <= 1f)
        {
            //BroadcastMessageAnimator.Play("BroadcastMessagePanelFadeOut",0,0f);
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
        Debug.Log("下一波敵人即將來襲!" + waveNumber);
        Wave wave = EnemyWaves[waveNumber];
        EnemyContent[] enemyContent = wave.Enemy;
        GameObject.Find("GameControl").GetComponent<PlayerStats>().Rounds++;
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
        // ===== 計算敵人總數 =====
        foreach (var e in enemyContent)
        {
            TotalEnemyCount += e.spawnCount;
        }
        waveNumber++;
        // ===== 開始生成敵人 =====
        foreach (var e in enemyContent)
        {
            for (int i = 0; i < e.spawnCount; i++)
            {
                while (!GameObject.Find("GameControl").GetComponent<GameManager>().isGamePlaying)
                {
                    yield return new WaitForSeconds(0.1f);
                }
                SpawnEnemy(e);
                yield return new WaitForSeconds(1f / e.spawnRate);
            }
            yield return new WaitForSeconds(e.delayToNextContent);
        }
    }
    void SpawnEnemy(EnemyContent enemyContent)
    {
        GameObject enemy = Instantiate(enemyContent.Enemy, spawnPoint.position, Quaternion.Euler(new Vector3(0, 270, 0)));
        Enemies enemyStatus = enemy.GetComponent<Enemies>();
        EnemyAttack enemyAttackScript = enemy.GetComponent<EnemyAttack>();
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
        if (spawnedCount == 2)
        {
            TextControl.BroadcastControlMessage("tutorial/text1");
        }
    }
}