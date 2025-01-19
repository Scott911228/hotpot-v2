using UnityEngine;

public class HealCharacter : MonoBehaviour
{
    Animator myAnimator;
    private Transform target;
    public bool isDead = false;
    public bool isPaused = true;

    [Header("Attributes")]
    public float rangeLength = 15f; // 攻擊範圍的長度
    public float attackWidth = 1f; // 攻擊範圍的寬度
    public float attackHeight = 1f; // 攻擊範圍的高度
    public bool showAttackRange = false; // 控制是否顯示攻擊範圍
    public string characterType;
    public float fireRate = 1f;
    private float fireCountdown = 0f;

    [Header("Unity Setup Fields")]
    public Transform CharacterTransform; // 主角色物件
    public Transform Rotate_Head;
    public float turnSpeed = 10f;

    public GameObject bulletPrefab;
    public Transform firePoint;

    public CharacterHP characterToHeal; // 用來指定需要治療的角色
    public float healAmount = 10f; // 每次治療的數量

    private Quaternion fixedCharacterRotation; // 固定角色朝向

    void Start()
    {
        myAnimator = GetComponent<Animator>();
        InvokeRepeating("UpdateTarget", 0.05f, 0.5f);
        fixedCharacterRotation = CharacterTransform.rotation; // 初始化固定朝向
    }

    void UpdateTarget()
    {
        // 將目標設置為 Character1
        if (characterToHeal != null && characterToHeal.GetCurrentHP() < characterToHeal.StartHealth)
        {
            target = characterToHeal.transform; // 將目標設置為需要治療的角色
        }
        else
        {
            target = null; // 沒有目標時設置為 null
        }
    }

    void Update()
    {
        if (target == null)
            return;

        if (isPaused || isDead) return;

        // 計算攻擊方向
        Vector3 dir = target.position - transform.position;
        float distanceToTarget = dir.magnitude; // 計算距離 確定目標在角色範圍內
        Quaternion lookRotation = Quaternion.LookRotation(dir);

        // 更新角色的固定朝向（僅在檢查目標時）
        CharacterTransform.rotation = fixedCharacterRotation; // 保持角色固定朝向

        // 只有當目標在範圍內時才進行治療
        if (distanceToTarget <= rangeLength)
        {
            if (fireCountdown <= 0f)
            {
                Heal();
                fireCountdown = 1f / fireRate;
            }
            fireCountdown -= Time.deltaTime;
        }
    }

    void Heal()
    {
        characterToHeal.ReceiveHealing(healAmount);
        myAnimator.SetTrigger("heal");
    }

    public void SetCharacterDirection(Quaternion direction)
    {
        CharacterTransform.rotation = direction; // 根據需要更新角色的旋轉
    }

    private void OnDrawGizmos()
    {
        if (!showAttackRange) return; // 如果不顯示，則退出

        Gizmos.color = Color.red;

        // 設定旋轉角度，這裡使用 Rotate_Head 的旋轉
        Quaternion rotation = Rotate_Head != null ? Rotate_Head.rotation : Quaternion.identity;

        // 設定攻擊範圍的起始點（角色的位置）
        Vector3 startPosition = firePoint.position;

        // 設定攻擊範圍的長度
        float length = rangeLength; // 攻擊範圍的長度

        // 創建一個矩陣，應用旋轉和位移
        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(startPosition, rotation, Vector3.one);

        // 繪製從角色前方延伸的長方體
        Gizmos.DrawWireCube(Vector3.forward * (length / 2), new Vector3(attackWidth, attackHeight, length)); // 使用 attackWidth 和 attackHeight

        // 恢復之前的矩陣
        Gizmos.matrix = oldMatrix;
    }
}
