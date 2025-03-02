using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Character : MonoBehaviour
{
    Animator myAnimator;
    private Transform target;

    private List<GameObject> targets = new List<GameObject>();
    public bool isDead = false;
    public bool isPaused = true;

    [Header("Attributes")]
    public float attackRadius = 1f; // 攻擊範圍的長度
    public float attackLength = 15f; // 攻擊範圍的長度
    public float attackWidth = 1f; // 攻擊範圍的寬度
    public float attackHeight = 1f; // 攻擊範圍的高度
    public bool showAttackRange = false; // 控制是否顯示攻擊範圍
    public bool canSkipDirectionSelection = false; // 是否跳過方向選擇
    public string characterType;
    public string characterClass;
    public float fireCooldown = 1f;
    private float fireCountdown = 0f;

    [Header("Unity Setup Fields")]
    public string characterTag = "Character";
    public string enemyTag = "Enemy";
    public Transform CharacterTransform; // 主角色物件
    public Transform Rotate_Head;
    public float turnSpeed = 10f;

    public GameObject bulletPrefab;
    public GameObject healbulletPrefab;
    public Transform firePoint;

    private Quaternion fixedCharacterRotation; // 固定角色朝向

    void Start()
    {
        myAnimator = GetComponent<Animator>();
        // ======= 湯底效果 =======
        fireCooldown /= GameObject.Find("LevelSettings").GetComponent<LevelSettings>().CharacterAttackSpeedMultiplier;
        // =======================
        InvokeRepeating("UpdateTarget", 0.05f, 0.05f);
        fixedCharacterRotation = CharacterTransform.rotation; // 初始化固定朝向
    }
    void UpdateTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        targets.Clear();
        foreach (GameObject enemy in enemies)
        {
            //enemy.GetComponent<SpriteRenderer>().color = Color.white;
        }
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;
        Vector3 colliderStartPos = firePoint.position;

        // 讓碰撞箱切入地面（暫時測試數值）
        colliderStartPos.y -= 2.5f;
        bool hasTarget = false;

        if (characterType != "Wall")
        {
            Collider[] hitColliderSphere = Physics.OverlapSphere(
                colliderStartPos,
                attackRadius);

            foreach (Collider hitCollider in hitColliderSphere)
            {
                if (hitCollider.gameObject.GetComponent<Enemies>())
                {

                    //hitCollider.gameObject.GetComponent<SpriteRenderer>().color = Color.yellow;
                    Vector3 directionToEnemy = hitCollider.gameObject.transform.position - transform.position;
                    float distanceToEnemy = directionToEnemy.magnitude;

                    if (distanceToEnemy < shortestDistance)
                    {
                        shortestDistance = distanceToEnemy;
                        nearestEnemy = hitCollider.gameObject;
                    }
                }
            }
            // 只在範圍內設置目標
            if (nearestEnemy != null)
            {
                target = nearestEnemy.transform;
                hasTarget = true;
            }
            else
            {
                target = null; // 沒有目標時設置為 null
            }
        }
        if (!hasTarget || characterType == "Wall")
        {

            // 依照角色朝向偏移碰撞箱
            switch (firePoint.eulerAngles.y)
            {
                case 0.0f:
                    colliderStartPos.x += attackLength;
                    break;
                case 90.0f:
                    colliderStartPos.z -= attackLength;
                    break;
                case 180.0f:
                    colliderStartPos.x -= attackLength;
                    break;
                case 270.0f:
                    colliderStartPos.z += attackLength;
                    break;
                default:
                    break;
            }
            Collider[] hitColliderBox = Physics.OverlapBox(
                colliderStartPos,
                new Vector3(attackWidth, attackHeight, attackLength),
                firePoint.rotation);
            // 尋找敵人


            foreach (Collider hitCollider in hitColliderBox)
            {
                /* // 攻擊範圍 Debug
                Node targetNode = hitCollider.GetComponent<Node>();
                if (targetNode != null)
                {
                    if (characterType != "Wall")
                    {
                        if (targetNode.tag == characterType) targetNode.GetComponent<Node>().ChangeNodeColor(targetNode, Color.yellow); // 將 Node 改為綠色
                    }
                    else
                    {
                        if (targetNode.tag == "Road") targetNode.GetComponent<Node>().ChangeNodeColor(targetNode, Color.yellow);
                    }
                }*/
                if (characterClass == "Healer")
                {
                    if (hitCollider.gameObject.GetComponent<Character>())
                    {
                        targets.Add(hitCollider.gameObject);
                    }
                }
                else
                {
                    if (hitCollider.gameObject.GetComponent<Enemies>())
                    {

                        //hitCollider.gameObject.GetComponent<SpriteRenderer>().color = Color.yellow;
                        Vector3 directionToEnemy = hitCollider.gameObject.transform.position - transform.position;
                        float distanceToEnemy = directionToEnemy.magnitude;

                        if (distanceToEnemy < shortestDistance)
                        {
                            shortestDistance = distanceToEnemy;
                            nearestEnemy = hitCollider.gameObject;
                        }
                    }
                }
            }
            if (characterClass == "Healer")
            {
            }
            else
            {
                // 只在範圍內設置目標
                if (nearestEnemy != null)
                {
                    target = nearestEnemy.transform;
                }
                else
                {
                    target = null; // 沒有目標時設置為 null
                }
            }

        }

    }


    void Update()
    {
        if (characterClass == "Healer") // 治療範圍對象
        {
            if (fireCountdown <= 0f &&
                !isPaused &&
                GameObject.Find("GameControl").GetComponent<GameManager>().isGamePlaying)
            {
                if (targets.ToArray().Length > 0)
                {
                    bool flag = false;
                    foreach (GameObject target in targets)
                    {
                        CharacterHP targetHP = target.GetComponent<CharacterHP>();
                        if (targetHP.GetCurrentHP() < targetHP.StartHealth)
                        {
                            flag = true;
                        }
                    }
                    if (flag) // 如果有可以能治療的對象
                    {
                        PlayHealAnim();
                        fireCountdown = fireCooldown;
                    }
                }
            }
            fireCountdown -= Time.deltaTime;
        }
        else // 單體對象
        {
            if (target == null)
                return;
            if (isDead) return;
            // 計算攻擊方向
            Vector3 dir = target.position - transform.position;
            // 角色轉向
            Vector3 flipedXScale = transform.localScale;
            if (dir.z < 0) flipedXScale.x = Math.Abs(flipedXScale.x) * -1;
            else flipedXScale.x = Math.Abs(flipedXScale.x);
            transform.localScale = flipedXScale;
            // 只有當目標在攻擊範圍內時才進行攻擊
            if (fireCountdown <= 0f &&
                !isPaused &&
                GameObject.Find("GameControl").GetComponent<GameManager>().isGamePlaying)
            {
                PlayAttackAnim();
                fireCountdown = fireCooldown;
            }
            fireCountdown -= Time.deltaTime;
        }
    }

    void PlayHealAnim() // 播放攻擊動畫
    {
        myAnimator.SetTrigger("heal");
    }
    void Heal()
    {
        // 添加 Debug.Log 來追蹤調用
        foreach (GameObject target in targets)
        {
            GameObject healingBulletGO = Instantiate(healbulletPrefab, firePoint.position, firePoint.rotation);
            HealingBullets healingBullet = healingBulletGO.GetComponent<HealingBullets>();
            Debug.Log("Healing " + target);
            if (healingBullet != null)
            {
                healingBullet.Seek(target.transform);
            }
        }
    }
    void PlayAttackAnim() // 播放攻擊動畫
    {
        myAnimator.SetTrigger("attack");
    }
    void DoDamage() // 從攻擊動畫中調用此函數
    {
        GameObject bulletGO = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Bullets bullets = bulletGO.GetComponent<Bullets>();
        if (bullets != null)
        {
            bullets.Seek(target);
            target = null;
        }
    }

}
