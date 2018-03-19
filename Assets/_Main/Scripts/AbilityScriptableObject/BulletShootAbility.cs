using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Ability/BulletShootAbility", order = 2)]
public class BulletShootAbility : AbilitySO {

    public float bulletSpeed;
    public GameObject bulletPrefab;

    private BulletShootTriggerable blShooter;

	// Use this for initialization
	public override void Initialize (GameObject gameObject) {
        blShooter = gameObject.GetComponent<BulletShootTriggerable>();

        if (blShooter == null) return;

        blShooter.bulletLiveTime = range/bulletSpeed;
        blShooter.bulletSpeed = bulletSpeed;
        blShooter.bulletPrefab = bulletPrefab;

    }
	
	// Update is called once per frame
	public override void Fire () {
		if(blShooter!=null)
        {
            blShooter.Fire();
        }
	}
}
