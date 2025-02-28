using Fungus;
using UnityEngine;
using UnityEngine.UI;

public class Enemies : MonoBehaviour
{
    private Animator animator;
    public HealthBar HealthBar;
    private Transform blocker;
    public float initSpeed;
    public float speedMultiplier = 1.0f;
    private float currentSpeed;
    public bool isPaused = false;
    private bool isBlocked = false; // 敵人是否被角色阻擋

    public float StartHealth = 100;
    public float Health;

    public int GetMoney = 50;

    [Header("Attributes")]
    public float range = 6f; // 受角色阻擋的距離

    [Header("Unity Setup Fields")]
    public string characterTag = "Character";
    public string enemyTag = "Enemy";

    [Header("Unity Stuff")]
    private bool isDead = false;

    private Transform Target;

    private int wavepointIndex = 0;

    private bool isSlowed = false;
    private bool isPoisoned = false;
    private float slowMultiplier;
    private int poisonDamage;
    private float poisonTime;
    private float slowTime;
    void Start()
    {
        animator = GetComponent<Animator>();
        Target = Paths.points[0];
        // ======= 湯底效果 =======
        StartHealth *= GameObject.Find("LevelSettings").GetComponent<LevelSettings>().EnemyHealthMultiplier;
        speedMultiplier *= GameObject.Find("LevelSettings").GetComponent<LevelSettings>().EnemyMovementMultiplier;
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
            //gameObject.GetComponent<SpriteRenderer>().color = Color.green;
            if (poisonTime > 0.4f)
            {
                TakeDamage(poisonDamage, 25f, new Color(0.2460261f, 0.1361694f, 0.6415094f, 1f), "中毒! ");
                poisonTime -= 0.4f;
            }
            else
            {
                isPoisoned = false;
                //gameObject.GetComponent<SpriteRenderer>().color = Color.white;
            }
        }
        else
        {
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
        PlayerStats.Money += GetMoney;
        Destroy(gameObject);
        WaveSpawn.EnemiesAlive--;
        WaveSpawn.KilledEnemyCount++;
    }
    void Update()
    {
        if (isPaused) return;
        Vector3 dir = Target.position - transform.position;
        transform.Translate(dir.normalized * currentSpeed * Time.deltaTime * (isBlocked ? 0 : 1), Space.World);
        if (Vector3.Distance(transform.position, Target.position) <= 0.2f)
        {
            GetNextWaypoint();
        }
    }

    void GetNextWaypoint()
    {
        if (wavepointIndex >= Paths.points.Length - 1)
        {
            EndPath();
            return;
        }
        wavepointIndex++;
        Target = Paths.points[wavepointIndex];
    }

    void EndPath()
    {
        if (PlayerStats.Life > 0) PlayerStats.Life--;
        WaveSpawn.EnemiesAlive--;
        WaveSpawn.KilledEnemyCount++;
        Destroy(gameObject);

    }
}