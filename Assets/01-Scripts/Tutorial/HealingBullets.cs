using UnityEngine;

public class HealingBullets : MonoBehaviour
{
    private Transform target; // 目標

    public float speed = 1000f; // 子彈速度
    public float healamount = 200f; //治療量
    public GameObject ParticlesEffect; // 粒子效果

    public bool isProtecting = false;

    public void Seek(Transform _target)
    {
        target = _target; // 設置目標
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject); // 如果目標為 null，銷毀子彈
            return;
        }

        Vector3 dir = target.position - transform.position; // 計算方向
        float distanceThisFrame = speed * Time.deltaTime; // 計算每幀移動距離

        if (dir.magnitude <= distanceThisFrame)
        {
            HitTarget(); // 到達目標
            return;
        }

        transform.Translate(dir.normalized * distanceThisFrame, Space.World); // 移動子彈
        transform.LookAt(target); // 旋轉子彈朝向目標
    }

    void HitTarget()
    {
        GameObject effectIns = Instantiate(ParticlesEffect, transform.position, transform.rotation);
        Destroy(effectIns, 5f);

        // 這裡需要確認目標是 `CharacterHP`
        CharacterHP targetHP = target.GetComponent<CharacterHP>();
        if (targetHP != null)
        {
            targetHP.ReceiveHealing(healamount); //治療量
            if(isProtecting) targetHP.AddProtect(0.4f, 3);
        }

        Destroy(gameObject);
    }

}
