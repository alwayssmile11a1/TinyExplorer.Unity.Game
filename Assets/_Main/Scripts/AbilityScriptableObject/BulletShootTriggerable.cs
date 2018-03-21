using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletShootTriggerable : MonoBehaviour {

    //these variable will be driven by RayCastWeaponSO script
    [HideInInspector] public float bulletSpeed;
    [HideInInspector] public float bulletLiveTime;
    [HideInInspector] public GameObject bulletPrefab;
    [HideInInspector] public float fireRate;

    public Transform startingShootPosition;

    private float m_Timer;
    private bool m_CanShoot = true;

    private void Update()
    {

        if (m_Timer > 0)
        {
            m_Timer -= Time.deltaTime;
        }
    }


    public void Trigger()
    {
        if (!m_CanShoot) return;

        if (m_Timer > 0) return;

        GameObject bullet = Instantiate(bulletPrefab, startingShootPosition.position, startingShootPosition.rotation);
 
        StartCoroutine(TranslateBullet(bullet));

        Destroy(bullet, bulletLiveTime);

        //reset timer
        m_Timer = 1 / fireRate;
    }

    IEnumerator TranslateBullet(GameObject bullet)
    {
        while (bullet!=null)
        {
            bullet.transform.Translate(bullet.transform.right * bulletSpeed * Time.deltaTime, Space.World);
            yield return null;
        }
    }

    public void SetCanShoot(bool canShoot)
    {
        m_CanShoot = canShoot;
    }


}
