using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamekit2D;
using BTAI;
public class ExoticPlant : MonoBehaviour
{


    public GameObject bullet;
    public float timeBetweenShooting = 0.5f;
    public float fireTime = 0.5f;
    public float startDelay = 0.5f;
    public float bulletSpeed = 5f;

    [Header("Misc")]
    public RandomAudioPlayer shootAudioPlayer;
    public string hitEffect = "Hit";
    public string deadEffect = "Dead";

    private Animator m_Animator;
    private float m_ShootingTimer;
    private BulletPool m_BulletPool;

    private int m_HashAttackPara = Animator.StringToHash("Attack");

    private int m_HashHitEffect;
    private int m_HashDeadEffect;

    private Root m_AI = BT.Root();




    // Use this for initialization
    void Awake()
    {

        m_HashHitEffect = VFXController.StringToHash(hitEffect);
        m_HashDeadEffect = VFXController.StringToHash(deadEffect);

        m_Animator = GetComponent<Animator>();

        m_BulletPool = BulletPool.GetObjectPool(bullet, 2);


        m_AI.OpenBranch(
            BT.Call(() => m_ShootingTimer = fireTime),
            BT.Call(() => m_Animator.SetBool(m_HashAttackPara, true)),
            BT.WaitUntil(() => m_ShootingTimer <= 0),
            BT.Call(() => m_Animator.SetBool(m_HashAttackPara, false)),
            BT.WaitForAnimatorState(m_Animator, "ExoticPlant_Idle"),
            BT.Wait(timeBetweenShooting)
        );



    }

    // Update is called once per frame
    void Update()
    {

        if (startDelay > 0)
        {
            startDelay -= Time.deltaTime;
        }
        else
        {
            m_AI.Tick();
        }


        if (m_ShootingTimer > 0)
        {
            m_ShootingTimer -= Time.deltaTime;

        }



    }

    public void Shoot()
    {
        BulletObject bulletObject = m_BulletPool.Pop(transform.position + transform.up * 0.8f);
        bulletObject.transform.RotateToDirection(transform.up);
        bulletObject.rigidbody2D.velocity = bulletSpeed * transform.up;
        if (shootAudioPlayer != null)
            shootAudioPlayer.PlayRandomSound();
    }

    public void GotHit()
    {
        VFXController.Instance.Trigger(m_HashHitEffect, transform.position, 0, false, null);

    }

    public void Die()
    {
        VFXController.Instance.Trigger(m_HashDeadEffect, transform.position, 0, false, null);
        gameObject.SetActive(false);
    }

}
