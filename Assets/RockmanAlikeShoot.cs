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
    [SerializeField]
    private int shootAngle;
    private int angleToTarget;

    //public float BulletExistsTime;
    public GameObject Bullet;
    private Transform BulletPostion;
    private StartShooting BulletShootingScript;
    private float currentExistsTime;

    private Animator animator;
    private BulletPool bulletPool;
    private StartShooting shootScript;

    // Use this for initialization
    void Awake () {
        shootTime = shootCoolDown;
        contactFilter2D.layerMask = hitLayerMask;
        rayCastPosition = transform.position;
        bulletPool = BulletPool.GetObjectPool(Bullet, 10);
        animator = GetComponent<Animator>();
        shootScript = Bullet.GetComponent<StartShooting>();

    }
	
	// Update is called once per frame
	void Update () {
        ScanForPlayer();
        CoolDownShoot();
	}
    public void ScanForPlayer()
    {
        RaycastHit2D[] hit = new RaycastHit2D[1];
        shootAngle = 0;
        Vector2 direction;
        for (int i = 0; i < 3; i++)
        {
        direction = (Quaternion.Euler(0, 0, shootAngle) * Vector2.left).normalized;
            Physics2D.Raycast(rayCastPosition, direction, contactFilter2D, hit, castDistance);
            Debug.DrawRay(rayCastPosition, direction * castDistance);
            if (hit[0].collider != null && shootTime >= shootCoolDown)
            {
                Debug.Log(hit[0].collider.tag);
                shootTime -= shootCoolDown;
                angleToTarget = shootAngle;
                animator.SetBool("attack", true);
            }
            shootAngle -= 30;
        }
    }
    public void Shooting()
    {
        animator.SetBool("attack", false);
        //pop instantiate at first call
        BulletObject bullet = bulletPool.Pop(shootOrigin.position);
        bullet.instance.GetComponent<StartShooting>().direction = (Quaternion.Euler(0, 0, angleToTarget) * Vector2.left).normalized;
        Debug.Log(animator.GetBool("attack"));
        StartCoroutine(test());
    }
    public void CoolDownShoot()
    {
        if (shootTime <= shootCoolDown)
            shootTime += Time.deltaTime;
    }

    public void OnGetDamage()
    {
        Debug.Log("get damage");
    }

    public void OnDie()
    {
        Debug.Log("die");
    }

    private IEnumerator test()
    {
        yield return new WaitForSeconds(0.3f);
        shootScript.direction = new Vector2(-1, 0);
    }
}
