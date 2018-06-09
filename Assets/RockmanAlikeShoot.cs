using Gamekit2D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockmanAlikeShoot : MonoBehaviour {
    public LayerMask hitLayerMask;
    private ContactFilter2D contactFilter2D;
    public float castDistance;
    private Vector2 rayCastPosition;
    public Vector2 castDirection = Vector2.left;
    [Range(1,3)]
    public int numberOfBullet;

    public float shootCoolDown;
    public Transform shootOrigin;
    public Transform shootOriginRight;
    private float shootTime;
    [SerializeField]
    private int shootAngle;
    private int angleToTarget;

    //public float BulletExistsTime;
    public GameObject Bullet;
    private Transform BulletPostion;
    private StartShooting BulletShootingScript;
    private float currentExistsTime;

    public ParticleSystem hitEffect;
    int DieEffectHash;

    private Animator animator;
    private BulletPool bulletPool;
    private BulletObject[] bulletObjects;
    private StartShooting shootScript;
    private SpriteRenderer spriteRenderer;
    private UpdateHealthBar updateHealthBar;
    // Use this for initialization
    void Awake () {
        DieEffectHash = VFXController.StringToHash("Smoke");
        shootTime = shootCoolDown;
        contactFilter2D.layerMask = hitLayerMask;
        rayCastPosition = transform.position;
        bulletPool = BulletPool.GetObjectPool(Bullet, 10);
        animator = GetComponent<Animator>();
        shootScript = Bullet.GetComponent<StartShooting>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        updateHealthBar = GetComponentInChildren<UpdateHealthBar>();
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
        direction = (Quaternion.Euler(0, 0, shootAngle) * castDirection).normalized;
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
        bulletObjects = new BulletObject[numberOfBullet];
        if (spriteRenderer.flipX)
        {
            angleToTarget += 240;
            StartCoroutine(PopBullet(shootOriginRight.position));
        }
        else
        {
            StartCoroutine(PopBullet(shootOrigin.position));
        }

        //if (bullet == null)
        //    return;
        Debug.Log("angle to target: " + angleToTarget);
        //bullet.instance.GetComponent<StartShooting>().direction = (Quaternion.Euler(0, 0, angleToTarget) * Vector2.left).normalized;
        Debug.Log(animator.GetBool("attack"));
        StartCoroutine(ResetBullet());
    }
    private IEnumerator PopBullet(Vector3 pos)
    {
        for (int i = 0; i < bulletObjects.Length; ++i)
        {
            bulletObjects[i] = bulletPool.Pop(pos);
            bulletObjects[i].instance.GetComponent<StartShooting>().direction = (Quaternion.Euler(0, 0, angleToTarget) * Vector2.left).normalized;
            yield return new WaitForSeconds(0.1f);
        }
    }
    public void CoolDownShoot()
    {
        if (shootTime <= shootCoolDown)
            shootTime += Time.deltaTime;
    }

    public void OnGetDamage()
    {
        hitEffect.Play();
    }

    public void OnDie()
    {
        //dieEfect.Play();
        //StartCoroutine(WaitToDisable());
        gameObject.SetActive(false);
        VFXController.Instance.Trigger(DieEffectHash, transform.position, 0, false, null, null);
    }
    private IEnumerator ResetBullet()
    {
        yield return new WaitForSeconds(0.3f);
        shootScript.direction = new Vector2(-1, 0);
    }
}
