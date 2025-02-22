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
        if (EnemiesAlive > 0)
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
        Wave wave = EnemyWaves[waveNumber];
        waveNumber++;
        KilledEnemyCount = 0;
        TotalEnemyCount = wave.count;
        Debug.Log("下一波敵人即將來襲!");
        if (GameObject.Find("LevelSettings").GetComponent<LevelSettings>().StageName == "第二關")
        {
            if (waveNumber == 2)
            {
                TextControl.BroadcastControlMessage("tutorial2/text3");
            }
            if (waveNumber == 3)
            {
                TextControl.BroadcastControlMessage("tutorial2/text5");
            }
        }
        for (int i = 0; i < wave.count; i++)
        {
            while (!GameObject.Find("GameControl").GetComponent<GameManager>().isGamePlaying)
            {
                yield return new WaitForSeconds(0.1f);
            }
            SpawnEnemy(wave.Enemy);
            yield return new WaitForSeconds(1f / wave.rate);
        }

        GameObject.Find("GameControl").GetComponent<PlayerStats>().Rounds++;
    }
    void SpawnEnemy(GameObject enemies)
    {
        Instantiate(enemies, spawnPoint.position, Quaternion.Euler(new Vector3(0, 270, 0)));
        EnemiesAlive++;
        spawnedCount++;
        if (spawnedCount == 2)
        {
            TextControl.BroadcastControlMessage("tutorial/text1");
        }
    }
}