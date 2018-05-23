using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamekit2D;

public class ShootingBase : MonoBehaviour {

    public GameObject bullet;
    public float timeBetweenShooting = 0.5f;
    public float startDelay = 0.5f;
    public float bulletSpeed = 5f;


    private float m_ShootingTimer;
    private BulletPool m_BulletPool;



    // Use this for initialization
    void Awake()
    {

        m_BulletPool = BulletPool.GetObjectPool(bullet, 2);
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

            if (m_ShootingTimer > 0)
            {
                m_ShootingTimer -= Time.deltaTime;
            }
            else
            {
                Shoot();
                m_ShootingTimer = timeBetweenShooting;
            }
        }





    }

    public void Shoot()
    {
        BulletObject bulletObject = m_BulletPool.Pop(transform.position);
        bulletObject.transform.RotateToDirection(transform.up);
        bulletObject.rigidbody2D.velocity = bulletSpeed * transform.up;
    }
}
