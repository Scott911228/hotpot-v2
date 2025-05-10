using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;

public class Enemies : MonoBehaviour
{
    private Animator animator;
    public HealthBar HealthBar;
    private Transform blocker;
    public float initSpeed;
    public float speedMultiplier = 1.0f;
    private float currentSpeed;
    public bool isPaused = false;
    private bool isBlocked = false; //敵人是否被角色阻擋
    private bool EnteredBase = false; //是否進入我方保護點
    private SpriteRenderer spriteRenderer; //用來調控敵人透明度用的
    public float StartHealth = 100;
    public float Health;
    public int AttackPlayerBaseDamage = 1;
    public int GetMoney = 50;

    [Header("Attributes")]
    public float range = 6f;

    [Header("Unity Setup Fields")]
    public string characterTag = "Character";
    public string enemyTag = "Enemy";

    [Header("Unity Stuff")]
    private bool isDead = false;

    public Transform Target;
    private int wavepointIndex = 0;

    private bool isSlowed = false;
    private bool isPoisoned = false;
    private float slowMultiplier;
    private int poisonDamage;
    private float poisonTime;
    private float slowTime;
    // 路徑管理
    private Transform[] movingPath;
    public int pathIndex;
    private PlayerStats playerStats;
    void Start()
    {
        animator = GetComponent<Animator>();
        // ======= 湯底效果 =======
        StartHealth *= GameObject.Find("LevelSettings").GetComponent<LevelSettings>().EnemyHealthMultiplier;
        speedMultiplier *= GameObject.Find("LevelSettings").GetComponent<LevelSettings>().EnemyMovementMultiplier;
        playerStats = GameObject.Find("GameControl").GetComponent<PlayerStats>();
        // =======================
        Health = StartHealth;
        InvokeRepeating("UpdateBlocker", 0f, 0.01f);
        InvokeRepeating("DamageEffectCheck", 0f, 0.4f);
        InvokeRepeating("MovementEffectCheck", 0f, 0.5f);
    }

    void MovementEffectCheck() // 更新阻擋此敵人的角色
    {
        if (isSlowed)
        {
            if (slowTime > 0.5f)
            {
                currentSpeed = initSpeed * speedMultiplier * slowMultiplier;
                slowTime -= 0.4f;
            }
            else
            {
                isSlowed = false;
            }
        }
        else
        {
            currentSpeed = initSpeed * speedMultiplier;
        }
    }

    void DamageEffectCheck()
    {
        if (isPoisoned)
        {
            if (poisonTime > 0.4f)
            {
                TakeDamage(poisonDamage, 25f, new Color(0.2460261f, 0.1361694f, 0.6415094f, 1f), "中毒! ");
                poisonTime -= 0.4f;
            }
            else
            {
                isPoisoned = false;
            }
        }
    }

    void UpdateBlocker() // 更新阻擋此敵人的角色
    {
        GameObject[] characters = GameObject.FindGameObjectsWithTag(characterTag);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestCharacter = null;
        if (characters.Length > 0)
        {
            foreach (GameObject characterObj in characters)
            {
                Character CharacterBlocker = characterObj.GetComponent<Character>();

                // 確保只有 CharacterType 為 'Road' 的角色能阻擋敵人
                if (CharacterBlocker != null && CharacterBlocker.characterType == "Road")
                {
                    float distanceToCharacter = Vector3.Distance(transform.position, characterObj.transform.position);
                    if (distanceToCharacter < shortestDistance)
                    {
                        shortestDistance = distanceToCharacter;
                        nearestCharacter = characterObj;
                    }
                }
            }

            // 如果找到符合條件的角色且在阻擋範圍內
            if (nearestCharacter != null && shortestDistance <= range)
            {
                blocker = nearestCharacter.transform;
                isBlocked = true;
            }
            else // 沒有找到符合條件的角色
            {
                blocker = null;
                isBlocked = false;
            }
        }
        else
        {
            blocker = null;
            isBlocked = false;
        }
    }

    public void AddSlow(float _slowMultiplier, float effectLength)
    {
        isSlowed = true;
        slowMultiplier = _slowMultiplier;
        slowTime = effectLength;
    }

    public void AddPoison(int damage, float effectLength)
    {
        isPoisoned = true;
        poisonDamage = damage;
        poisonTime = effectLength;
    }

    public void TakeDamage(int amount, float size, Color color, string prefix)
    {
        Health -= amount;
        GameObject.Find("GameControl").GetComponent<GameManager>().DisplayDamage(
            gameObject,
            prefix + amount.ToString(),
            size,
            color);
        HealthBar.UpdateHealthBar(StartHealth, Health);
        if (prefix == "")
        {
            animator.SetTrigger("damage");
        }
        if (Health <= 0 && !isDead)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        playerStats.Money += GetMoney;
        Destroy(gameObject);
        WaveSpawn.EnemiesAlive--;
        WaveSpawn.KilledEnemyCount++;
        GameStats.Instance.EnemyKilledCount++;
    }
    void Update()
    {
        if (isPaused) return;

        if (Target == null)
        {
            Debug.LogError($"敵人 {gameObject.name} 沒有有效的目標點！");
            return;
        }

        // 計算方向並移動
        Vector3 dir = Target.position - transform.position;
        float moveDistance = currentSpeed * Time.deltaTime * (isBlocked ? 0 : 1);

        if (dir.magnitude <= Mathf.Max(0.2f, moveDistance * 2))
        {
            GetNextWaypoint();
        }
        else
        {
            transform.Translate(dir.normalized * moveDistance, Space.World);
        }
    }
    void GetNextWaypoint()
    {
        if (movingPath == null || movingPath.Length == 0)
        {
            Debug.LogError($"敵人 {gameObject.name} 的路徑無效！");
            return;
        }

        if (wavepointIndex < movingPath.Length - 1)
        {
            wavepointIndex++;
            Target = movingPath[wavepointIndex];
            //Debug.Log($"敵人 {gameObject.name} 移動到下一個節點: {Target.name}");
        }
        else
        {
            EndPath();
        }
    }

    void EndPath()
    {
        if (EnteredBase) return; //防止重複進入基地扣血
        EnteredBase = true;

        if (PlayerStats.Life > 0) PlayerStats.Life -= AttackPlayerBaseDamage;
        if (PlayerStats.Life < 0) PlayerStats.Life = 0;
        WaveSpawn.EnemiesAlive--;
        WaveSpawn.KilledEnemyCount++;

        StartCoroutine(FadeOutAndDestroy()); //開始淡出消失
    }

    IEnumerator FadeOutAndDestroy()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        HealthBar healthBar = GetComponentInChildren<HealthBar>();

        if (spriteRenderer == null)
        {
            Destroy(gameObject);
            yield break;
        }

        Image healthBarImage = null;
        if (healthBar != null)
        {
            healthBarImage = healthBar.GetComponent<Image>();
        }

        float fadeDuration = 1.0f;
        float elapsedTime = 0f;

        Color startColor = spriteRenderer.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        Color startHealthColor = Color.white;
        if (healthBarImage != null)
        {
            startHealthColor = healthBarImage.color;
        }
        Color endHealthColor = new Color(startHealthColor.r, startHealthColor.g, startHealthColor.b, 0f);

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = 1f - (elapsedTime / fadeDuration);
            spriteRenderer.color = Color.Lerp(startColor, endColor, elapsedTime / fadeDuration);

            if (healthBarImage != null)
            {
                healthBarImage.color = Color.Lerp(startHealthColor, endHealthColor, elapsedTime / fadeDuration);
            }

            yield return null;
        }

        Destroy(gameObject);
    }
    public void InitializePath(Transform[] path)
    {
        if (path == null || path.Length == 0)
        {
            Debug.LogError("Path is invalid or empty!");
            return;
        }

        movingPath = path; // 讓敵人使用自己的路徑，而不是全局靜態變數
        wavepointIndex = 0;

        if (movingPath.Length > 0)
        {
            Target = movingPath[wavepointIndex];
            //Debug.Log($"敵人 {gameObject.name} 鎖定到下一個節點: {Target.name}");
        }
    }


}
