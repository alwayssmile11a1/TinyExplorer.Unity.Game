using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamekit2D;
using BTAI;
public class ExoticPlant : MonoBehaviour {


    public GameObject bullet;
    public float timeBetweenShooting = 0.5f;
    public float fireTime = 0.5f;
    public float startDelay = 0.5f;
    public float bulletSpeed = 5f;

    private Animator m_Animator;
    private float m_ShootingTimer;
    private BulletPool m_BulletPool;

    private int m_HashAttackPara = Animator.StringToHash("Attack");

    private Root m_AI = BT.Root();

	// Use this for initialization
	void Awake () {

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
	void Update () {

        if (startDelay > 0)
        {
            startDelay -= Time.deltaTime;
        }
        else
        {
            m_AI.Tick();
        }


        if(m_ShootingTimer>0)
        {
            m_ShootingTimer -= Time.deltaTime;

        }
        
       

	}

    public void Shoot()
    {
        BulletObject bulletObject = m_BulletPool.Pop(transform.position + transform.up * 0.8f);
        bulletObject.transform.RotateToDirection(transform.up);
        bulletObject.rigidbody2D.velocity = bulletSpeed * transform.up;
    }



}
