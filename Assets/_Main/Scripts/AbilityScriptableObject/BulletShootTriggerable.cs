using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletShootTriggerable : MonoBehaviour {

    //these variable will be driven by RayCastWeaponSO script
    [HideInInspector] public float bulletSpeed;
    [HideInInspector] public float bulletLiveTime;
    [HideInInspector] public GameObject bulletPrefab;

    public Transform startingShootPosition;
    


    public void Fire()
    {

    
        GameObject bullet = Instantiate(bulletPrefab, startingShootPosition.position, startingShootPosition.rotation);
        //Add a little bit of random
        bullet.transform.Rotate(Vector3.up, Random.Range(-5f, 5f), Space.Self);
        //bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward);

        StartCoroutine(TranslateBullet(bullet));

        Destroy(bullet, bulletLiveTime);


    }

    IEnumerator TranslateBullet(GameObject bullet)
    {
        while (bullet!=null)
        {
            bullet.transform.Translate(bullet.transform.forward * bulletSpeed * Time.deltaTime, Space.World);
            yield return null;
        }
    }




}
