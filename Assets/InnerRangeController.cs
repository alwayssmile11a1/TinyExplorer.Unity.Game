using Gamekit2D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InnerRangeController : MonoBehaviour {
    [Header("General")]
    public Transform targetToTrack;

    [Header("Attack1")]
    public Damager innerRangeDamager;
    public Damager attack1Damager;

    [Header("Attack2")]
    public ParticleSystem attack2FireParticle1;
    public ParticleSystem attack2FireParticle2;
    public ParticleSystem[] attack2PurpleExplodeParticles;
    public EdgeCollider2D[] edgeColliders;

    private SpriteRenderer spriteRenderer;
    // Use this for initialization
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    //// Update is called once per frame
    //void Update () {

    //}

    public void ActiveDamagerAndAttack2Par()
    {
        Debug.Log("Active damager");
        for (int i = 0; i < edgeColliders.Length; i++)
        {
            edgeColliders[i].enabled=true;
            if (spriteRenderer.flipX)
            {
                Vector2 offset = edgeColliders[i].offset;
                edgeColliders[i].offset = new Vector2(offset.x - 0.5f, offset.y);
            }
            attack2PurpleExplodeParticles[i].Play();
        }
    }
    public void DeactiveDamagers()
    {
        Debug.Log("Deactive damager");
        foreach (var item in edgeColliders)
        {
            if (spriteRenderer.flipX)
            {
                Vector2 offset = item.offset;
                item.offset = new Vector2(offset.x + 0.5f, offset.y);
            }
            item.enabled = false;
        }
    }
    public void ActiveAttack2FireParticle()
    {
        attack2FireParticle1.Play();
        attack2FireParticle2.Play();
        StartCoroutine(WaitToStopAttack2FireParticle());
    }
    
    public void ActiveAttack1Damager()
    {
        attack1Damager.EnableDamage();
    }
    public void DeactiveAttack1Damager()
    {
        attack1Damager.DisableDamage();
    }
    public void SetOffsetInnerRangeDamagerAT1()
    {
        Debug.Log("Set offset");
        Vector2 Offset = innerRangeDamager.offset;
        innerRangeDamager.offset = new Vector2(Offset.x + 1.5f, Offset.y);
    }
    public void ResetOffsetInnerRangeDamagerAT1()
    {
        Debug.Log("Reset offset");
        Vector2 Offset = innerRangeDamager.offset;
        innerRangeDamager.offset = new Vector2(Offset.x - 1.5f, Offset.y);
    }
    public void SetOffsetInnerRangeDamagerAT2()
    {
        Debug.Log("Set offset");
        Vector2 Offset = innerRangeDamager.offset;
        innerRangeDamager.offset = new Vector2(Offset.x -0.2f, Offset.y -1.5f);
    }
    public void ResetOffsetInnerRangeDamagerAT2()
    {
        Debug.Log("Reset offset");
        Vector2 Offset = innerRangeDamager.offset;
        innerRangeDamager.offset = new Vector2(Offset.x + 0.2f, Offset.y + 1.5f);
    }

    private IEnumerator WaitToStopAttack2FireParticle()
    {
        yield return new WaitForSeconds(2f);
        attack2FireParticle1.Stop();
        attack2FireParticle2.Stop();
    }
}
