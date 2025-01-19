using System;
using UnityEngine;

public class Bullets : MonoBehaviour
{
    private Transform target;

    public float speed = 70f;
    public bool isPoison = false;
    public bool isSlowing = false;
    public int damage = 50;
    public GameObject ParticlesEffect;

    public void Seek(Transform _target)
    {
        target = _target;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 dir = target.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        if (dir.magnitude <= distanceThisFrame)
        {
            HitTarget();
            return;
        }

        // 更新子彈位置
        transform.Translate(dir.normalized * distanceThisFrame, Space.World);

        // 設定子彈朝向 X 軸
        Quaternion targetRotation = Quaternion.LookRotation(Vector3.right, Vector3.up);
        transform.rotation = targetRotation;
    }

    void HitTarget()
{
    // 創建粒子特效並設置其位置和旋轉
    GameObject effectIns = (GameObject)Instantiate(ParticlesEffect, transform.position, Quaternion.LookRotation(Vector3.forward, Vector3.up));
    Destroy(effectIns, 5f);
    Damage(target);
    Destroy(gameObject);
}



    void Damage(Transform Enemies)
    {
        Enemies e = Enemies.GetComponent<Enemies>();
        int HeatLevel = GameObject.Find("GameControl").GetComponent<HeatControl>().HeatLevel;

        ///// 造成傷害
        e.TakeDamage(
            Convert.ToInt32(damage * (0.75 + HeatLevel * 0.25)),
            40,
            new Color(0.09411757f, 0.05882348f, 0.05882348f, 0.8705882f), "");
        
        ///// 毒子彈的話給效果
        if (isPoison) e.AddPoison(Convert.ToInt32(damage * (0.75 + HeatLevel * 0.25) / 8), 5f);
        
        ///// 減速子彈的話給效果
        if (isSlowing) e.AddSlow(0.3f, 4f);
    }
}
