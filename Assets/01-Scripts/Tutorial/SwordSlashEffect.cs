using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordSlashEffect : MonoBehaviour
{
    public ParticleSystem swordSlashParticle;

    public void PlayEffect(Vector3 position, Quaternion rotation)
    {
        ParticleSystem effect = Instantiate(swordSlashParticle, position, rotation);
        effect.Play();
        Destroy(effect.gameObject, effect.main.duration + effect.main.startLifetime.constantMax);
    }
}
