using Gamekit2D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackKnightBulletScript : MonoBehaviour {
    public ParticleSystem hitEffect;
    private FollowTarget followTarget;
    private Bullet bullet;
    private void Awake()
    {
        followTarget = GetComponent<FollowTarget>();
        bullet = GetComponent<Bullet>();
    }

    public void OnHit()
    {
        if (hitEffect != null)
            hitEffect.Play();
    }
    public void OnDie()
    {
        if (hitEffect != null)
            hitEffect.Play();
        StartCoroutine(DisabledAndReturnToPool());
    }

    private IEnumerator DisabledAndReturnToPool()
    {
        yield return new WaitForSeconds(hitEffect.main.duration - 0.4f);
        followTarget.enabled = false;
        bullet.ReturnToPool();
    }
}
