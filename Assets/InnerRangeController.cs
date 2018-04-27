using BTAI;
using Gamekit2D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InnerRangeController : MonoBehaviour {
    [Header("General")]
    public Transform targetToTrack;

    [Header("Attack1")]
    public Damager innerRangeDamager;
    public Damager[] attack1Damager;
    public float followSpeed;

    [Header("Attack2")]
    public ParticleSystem attack2FireParticle1;
    public ParticleSystem attack2FireParticle2;
    public ParticleSystem[] attack2PurpleExplodeParticles;
    public EdgeCollider2D[] edgeColliders;

    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private new Rigidbody2D rigidbody2D;

    Root innerRangeBT = BT.Root();
    // Use this for initialization
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rigidbody2D = GetComponent<Rigidbody2D>();

        innerRangeBT.OpenBranch(
            BT.SetBool(animator, "move", true),
            BT.WaitForAnimatorState(animator, "move"),
            BT.WaitUntil(CheckMoveToTarget),
            BT.SetBool(animator, "attack1", true),
            BT.SetBool(animator, "move", false),
            BT.WaitForAnimatorState(animator, "attack1"),
            BT.SetBool(animator, "attack1", false),
            BT.Call(Attack1),
            BT.Wait(5f)
        );
    }

    // Update is called once per frame
    void Update()
    {
        innerRangeBT.Tick();
    }
    
    private bool CheckMoveToTarget()
    {
        if ((transform.position - targetToTrack.position).sqrMagnitude <= 1)
        {
            rigidbody2D.velocity = Vector2.zero;
            return true;
        }
        if(transform.position.x < targetToTrack.position.x)
        {
            spriteRenderer.flipX = false;
        }
        else
        {
            spriteRenderer.flipX = true;
        }
        Vector2 direction = (targetToTrack.position - transform.position).normalized;
        rigidbody2D.velocity = direction * followSpeed;
        return false;
    }

    private void Attack1()
    {

    }

    #region Animation Event
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
    
    public void ActiveAttack1_1st2nd_Damager()
    {
        attack1Damager[0].EnableDamage();
    }
    public void ActiveAttack1_3rd_Damager()
    {
        attack1Damager[1].EnableDamage();
    }
    public void DeactiveAttack1Damager()
    {
        foreach (var item in attack1Damager)
        {
            item.DisableDamage();
        }
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
    #endregion
}
