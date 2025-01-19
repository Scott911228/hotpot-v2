using UnityEngine;

public class EnemyBullets : MonoBehaviour
{
    private Transform target;

    public float speed = 70f;

    public int damage = 200;
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

        transform.Translate(dir.normalized * distanceThisFrame, Space.World);
        transform.LookAt(target);
    }

    void HitTarget()
    {
        GameObject effectIns = (GameObject)Instantiate(ParticlesEffect, transform.position, transform.rotation);
        Destroy(effectIns, 5f);
        Damage(target);
        Destroy(gameObject);
    }

    void Damage(Transform Enemies)
    {
        CharacterHP e = Enemies.GetComponent<CharacterHP>();


        e.TakeDamage(damage);
        


    }
}
