using Gamekit2D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockmanAlikeShoot : MonoBehaviour {
    public LayerMask hitLayerMask;
    private ContactFilter2D contactFilter2D;
    public float castDistance;
    private Vector2 rayCastPosition;

    public float shootCoolDown;
    public Transform shootOrigin;
    private float shootTime;

    public float BulletExistsTime;
    public GameObject Bullet;
    private Transform BulletPostion;
    private StartShooting BulletShootingScript;
    private float currentExistsTime;

    private Animator animator;
    private BulletPool bulletPool;

    // Use this for initialization
    void Awake () {
        shootTime = shootCoolDown;
        contactFilter2D.layerMask = hitLayerMask;
        rayCastPosition = transform.position;
        bulletPool = BulletPool.GetObjectPool(Bullet, 10);
        animator = GetComponent<Animator>();

    }
	
	// Update is called once per frame
	void Update () {
        ScanForPlayer();
        CoolDownShoot();
	}
    public void ScanForPlayer()
    {
        RaycastHit2D[] hit = new RaycastHit2D[1];
        int angle = 0;
        Vector2 direction;
        for (int i = 0; i < 3; i++)
        {
        }
        Debug.Log("shoot time in Scan player: " + shootTime);
        Debug.Log("shoot cool down in Scan player: " + shootCoolDown);
        direction = (Quaternion.Euler(0, 0, angle) * Vector2.left).normalized;
            Physics2D.Raycast(rayCastPosition, direction, contactFilter2D, hit, castDistance);
            Debug.DrawRay(rayCastPosition, direction * castDistance);
            if (hit[0].collider != null && shootTime >= shootCoolDown)
            {
                Debug.Log(hit[0].collider.tag);
                shootTime -= shootCoolDown;
                animator.SetBool("attack", true);
            }
            angle -= 30;
    }
    public void Shooting()
    {
        Debug.Log("in shooting");
        animator.SetBool("attack", false);
        BulletObject bullet = bulletPool.Pop(shootOrigin.position);
        Debug.Log(animator.GetBool("attack"));
    }
    public void CoolDownShoot()
    {
        if (shootTime <= shootCoolDown)
            shootTime += Time.deltaTime;
        Debug.Log("shoot time in CoolDowm: " + shootTime);
    }

    public void OnGetDamage()
    {
        Debug.Log("get damage");
    }

    public void OnDie()
    {
        Debug.Log("die");
    }
}
