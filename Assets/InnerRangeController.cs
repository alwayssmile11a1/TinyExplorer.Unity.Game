using BTAI;
using Gamekit2D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InnerRangeController : MonoBehaviour {
    [Header("General")]
    public Transform targetToTrack;
    public ParticleSystem hitEffect;

    [Header("Attack1")]
    public Damager innerRangeDamager;
    public EdgeCollider2D[] attack1Damager;
    public float followSpeed;

    [Header("SpawnSphere")]
    public ParticleSystem spawnSphereEffect;
    public GameObject Sphere;
    public Transform[] spherePos;
    private BulletPool sphereBulletPool;
    private BulletObject[] sphereBulletObjects;

    [Header("Attack2")]
    public ParticleSystem attack2FireParticle1;
    public ParticleSystem attack2FireParticle2;
    public ParticleSystem[] attack2PurpleExplodeParticles;
    public EdgeCollider2D[] edgeColliders;
    public float moveToApproPosSpeed;

    [Header("Attack Thorn")]
    public GameObject ThornBullet;
    public GameObject LoadThorn;
    public Transform[] loadThornPos;
    private BulletPool thornLoadBulletPool;
    private BulletPool thornBulletPool;

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

        sphereBulletPool = BulletPool.GetObjectPool(Sphere, 5);
        thornLoadBulletPool = BulletPool.GetObjectPool(LoadThorn, 4);
        thornBulletPool = BulletPool.GetObjectPool(ThornBullet, 20);

        innerRangeBT.OpenBranch(
            BT.Sequence().OpenBranch(
                BT.SetBool(animator, "move", true),
                BT.WaitForAnimatorState(animator, "move"),
                BT.WaitUntil(CheckMoveToTarget),
                BT.SetBool(animator, "attack1", true),
                BT.SetBool(animator, "move", false),
                BT.WaitForAnimatorState(animator, "attack1"),
                BT.SetBool(animator, "attack1", false),
                BT.WaitForAnimatorState(animator, "stand")
            ),
            BT.Sequence().OpenBranch(
                BT.Call(() => spawnSphereEffect.Play()),
                BT.Wait(0.5f),
                BT.Call(PopSphere),
                BT.Wait(1.5f),
                BT.Call(SetSphereVelocity),
                BT.Wait(2f),
                BT.Call(() => spawnSphereEffect.Stop())
            ),
            BT.Sequence().OpenBranch(
                BT.SetBool(animator, "move", true),
                BT.WaitForAnimatorState(animator, "move"),
                BT.WaitUntil(CheckMoveToTarget),
                //BT.Call(MoveToAttack2Pos),
                BT.Call(ResetVelocity),
                BT.SetBool(animator, "attack2", true),
                BT.SetBool(animator, "move", false),
                BT.WaitForAnimatorState(animator, "attack2"),
                BT.SetBool(animator, "attack2", false),
                BT.WaitForAnimatorState(animator, "stand")
            ),
            BT.Sequence().OpenBranch(
                BT.Call(MoveToAttack2Pos),
                BT.Wait(1),
                BT.Call(ResetVelocity),
                BT.Call(PopLoadThorn),
                BT.Wait(1.5f),
                BT.Call(ShootThorns)
            )
        );
    }

    // Update is called once per frame
    void Update()
    {
        innerRangeBT.Tick();
    }
    #region Attack1
    private bool CheckMoveToTarget()
    {
        if ((transform.position - targetToTrack.position).sqrMagnitude <= 1.8)
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
            SetAttack1DamagerOffset();
        }
        Vector2 direction = (targetToTrack.position - transform.position).normalized;
        rigidbody2D.velocity = direction * followSpeed;
        return false;
    }
    private void SetAttack1DamagerOffset()
    {

    }
    private void Attack1()
    {

    }
    #endregion

    #region Attack Sphere
    private void PopSphere()
    {
        sphereBulletObjects = new BulletObject[spherePos.Length];
        for (int i = 0; i < spherePos.Length; ++i)
        {
            sphereBulletObjects[i] = sphereBulletPool.Pop(spherePos[i].position);
            sphereBulletObjects[i].instance.GetComponent<FallTowardTarget>().target = targetToTrack;
        }
    }

    private void SetSphereVelocity()
    {
        for (int i = 0; i < spherePos.Length; ++i)
        {
            sphereBulletObjects[i].instance.GetComponent<FallTowardTarget>().SetVelocity();
        }
        
    }
    #endregion

    #region Attack2
    private void MoveToAttack2Pos()
    {
        rigidbody2D.velocity = Vector2.up * moveToApproPosSpeed;
    }
    private void ResetVelocity()
    {
        rigidbody2D.velocity = Vector2.zero;
    }
    #endregion

    #region Attack Thorn
    private void PopLoadThorn()
    {
        Debug.Log("Pop Thorn");
        foreach (var item in loadThornPos)
        {
            thornLoadBulletPool.Pop(item.position);
        }
    }

    private void ShootThorns()
    {
        int angle = 0;
        Debug.Log("load thorn position: " + loadThornPos.Length);
        for (int i = 0; i < loadThornPos.Length; ++i)
        {
            for (int j = 0; j < 6; ++j)
            {
                BulletObject bulletObject = thornBulletPool.Pop(loadThornPos[i].position);
                bulletObject.instance.GetComponent<StartShooting>().direction = (Quaternion.Euler(0, 0, angle) * Vector2.right).normalized;
                bulletObject.instance.GetComponent<Transform>().rotation = Quaternion.Euler(0, 0, angle);
                angle = angle - 60 == -360 ? 0 : angle - 60;
                Debug.Log("thorn " + i + " :" + bulletObject.instance.GetComponent<Transform>().rotation.z);
            }
        }
    }

    

    #endregion

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
    
    public void ActiveAttack1_1st_Damager()
    {
        //attack1Damager[0].GetComponent<EdgeCollider2D>().enabled = true;
        attack1Damager[0].enabled = true;
        if (spriteRenderer.flipX)
        {
            Vector2 offset = attack1Damager[0].offset;
            attack1Damager[0].offset = new Vector2(-offset.x, offset.y);
        }
        //attack1Damager[0].offset
    }
    public void ActiveAttack1_2nd_Damager()
    {
        //attack1Damager[1].GetComponent<EdgeCollider2D>().enabled = true;
        attack1Damager[1].enabled = true;
        if (spriteRenderer.flipX)
        {
            Vector2 offset = attack1Damager[1].offset;
            attack1Damager[1].offset = new Vector2(-offset.x, offset.y);
        }
    }
    public void ActiveAttack1_3rd_Damager()
    {
        //attack1Damager[2].GetComponent<EdgeCollider2D>().enabled = true;
        attack1Damager[2].enabled = true;
        if (spriteRenderer.flipX)
        {
            Vector2 offset = attack1Damager[2].offset;
            attack1Damager[2].offset = new Vector2(-offset.x, offset.y);
        }
    }
    public void DeactiveAttack1Damager()
    {
        foreach (var item in attack1Damager)
        {
            //item.GetComponent<EdgeCollider2D>().enabled = false;
            item.enabled = false;
            if (spriteRenderer.flipX)
            {
                Vector2 offset = item.offset;
                item.offset = new Vector2(-offset.x, offset.y);
            }
        }
    }
    public void OnHit()
    {
        hitEffect.Play();
        animator.SetTrigger("hit");
    }
    public void OnDie()
    {
        gameObject.SetActive(false);
    }
    public void ResetHitTrigger()
    {
        animator.ResetTrigger("hit");
    }
    //public void SetOffsetInnerRangeDamagerAT1()
    //{
    //    Debug.Log("Set offset");
    //    Vector2 Offset = innerRangeDamager.offset;
    //    if (spriteRenderer.flipX)
    //    {
    //        innerRangeDamager.offset = new Vector2(Offset.x + 1.5f, Offset.y);
    //    }
    //    else
    //    {
    //        innerRangeDamager.offset = new Vector2(Offset.x - 1.5f, Offset.y);
    //    }
    //}
    //public void ResetOffsetInnerRangeDamagerAT1()
    //{
    //    Debug.Log("Reset offset");
    //    Vector2 Offset = innerRangeDamager.offset;
    //    if (spriteRenderer.flipX)
    //    {
    //        innerRangeDamager.offset = new Vector2(Offset.x - 1.5f, Offset.y);
    //    }
    //    else
    //    {
    //        innerRangeDamager.offset = new Vector2(Offset.x + 1.5f, Offset.y);
    //    }
    //}
    //public void SetOffsetInnerRangeDamagerAT2()
    //{
    //    Debug.Log("Set offset");
    //    Vector2 Offset = innerRangeDamager.offset;
    //    innerRangeDamager.offset = new Vector2(Offset.x -0.2f, Offset.y -1.5f);
    //}
    //public void ResetOffsetInnerRangeDamagerAT2()
    //{
    //    Debug.Log("Reset offset");
    //    Vector2 Offset = innerRangeDamager.offset;
    //    innerRangeDamager.offset = new Vector2(Offset.x + 0.2f, Offset.y + 1.5f);
    //}

    private IEnumerator WaitToStopAttack2FireParticle()
    {
        yield return new WaitForSeconds(2f);
        attack2FireParticle1.Stop();
        attack2FireParticle2.Stop();
    }
    #endregion
}
