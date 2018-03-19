using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Ability/BulletShootAbility", order = 2)]
public class BulletShootAbility : AbilitySO {

    public float fireRate = 5f;
    public float bulletSpeed = 5f;
    public GameObject bulletPrefab;

    private BulletShootTriggerable blShooter;

	// Use this for initialization
	public override void Initialize (GameObject gameObject) {
        blShooter = gameObject.GetComponent<BulletShootTriggerable>();

        if (blShooter == null) return;

        blShooter.bulletLiveTime = range/bulletSpeed;
        blShooter.bulletSpeed = bulletSpeed;
        blShooter.bulletPrefab = bulletPrefab;
        blShooter.fireRate = fireRate;

    }
	
	public override void Trigger () {
		if(blShooter!=null)
        {
            blShooter.Trigger();
        }
	}
}
