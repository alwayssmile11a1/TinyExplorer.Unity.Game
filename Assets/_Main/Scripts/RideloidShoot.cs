using Gamekit2D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RideloidShoot : MonoBehaviour {
    public LayerMask hitLayerMask;
    public float castDistance;

    public float shootCoolDown;
    public Transform shootOriginRight;
    public Transform shootOriginLeft;
    private float shootTime;
    //private bool Shooted;

    //public float RobotHandExistsTime;
    public GameObject RobotHand;
    private Transform RoBotHandPostion;
    private StartShooting robotHandShootingScript;
    //private float currentExistsTime;

    public ParticleSystem robotHitEffect;
    int DieEffectHash;

    private Vector2 direction;
    private ContactFilter2D contactFilter2D;
    private CapsuleCollider2D capsuleCollider2D;
    private new Rigidbody2D rigidbody2D;
    private Animator animator;
    private SpriteRenderer robotSpriteRenderer;
    private SpriteRenderer robotHandSpriteRenderer;
    private Vector2 offset;

    public Vector2 Direction
    {
        get
        {
            return direction;
        }

        set
        {
            direction = value;
        }
    }

    public Vector2 Offset
    {
        get
        {
            return offset;
        }

        set
        {
            offset = value;
        }
    }

    // Use this for initialization
    void Awake () {
        //currentExistsTime = 0;
        DieEffectHash = VFXController.StringToHash("Smoke");
        direction = Vector2.right;
        shootTime = shootCoolDown;
        //Shooted = false;
        contactFilter2D.layerMask = hitLayerMask;
        RoBotHandPostion = RobotHand.GetComponent<Transform>();
        robotHandShootingScript = RobotHand.GetComponent<StartShooting>();
        //RoBotHandPostion.position = shootOrigin.position;
        animator = GetComponent<Animator>();
        robotSpriteRenderer = GetComponent<SpriteRenderer>();
        robotHandSpriteRenderer = RobotHand.GetComponent<SpriteRenderer>();
        capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        Offset = new Vector2(capsuleCollider2D.size.x, 0);
	}
    

    private void FixedUpdate()
    {
        ScanForPlayer();
        CoolDownShoot();
        //ResetRobotHand();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag.Equals("RideloidRange"))
        {
            robotHandShootingScript.ReverseVelocity();
            robotHandSpriteRenderer.flipX = !robotHandSpriteRenderer.flipX;
        }
    }

    void ScanForPlayer()
    {
        RaycastHit2D[] hit = new RaycastHit2D[5];
        Vector2 Origin = rigidbody2D.position + Offset;
        Physics2D.Raycast(Origin, direction, contactFilter2D, hit, castDistance);
        //Debug.Log(hit);
        Debug.DrawRay(Origin , direction * castDistance);
        foreach (RaycastHit2D item in hit)
        {
            if (item.rigidbody != null && item.collider.tag.Equals("Player"))
            {
                ShootPlayer();
            }
        }
        //if (!foundPlayer && animator.GetBool("attack"))
        //    animator.SetBool("attack", false);
    }

    void ShootPlayer()
    {
        if (shootTime >= shootCoolDown)
        {
            Debug.Log("Shoot");
            animator.SetBool("attack", true);
        }
    }

    public void Shooting()
    {
        Debug.Log("in shooting");
        RobotHand.SetActive(true);
        if (!robotSpriteRenderer.flipX)
        {
            RoBotHandPostion.position = shootOriginRight.position;
        }
        else
        {
            RoBotHandPostion.position = shootOriginLeft.position;
        }
        shootTime -= shootCoolDown;
        animator.SetBool("attack", false);
        //Shooted = true;
        //Debug.Log(Time.deltaTime);
    }

    public void OnHit()
    {
        robotHitEffect.Play();
    }

    public void OnDie()
    {
        //robotDieEffect.Play();
        //StartCoroutine(WaitToDisable());
        //GetComponent<MovingInRange>().CanMove = false;
        gameObject.SetActive(false);
        VFXController.Instance.Trigger(DieEffectHash, transform.position, 0, false, null, null);
    }

    private void CoolDownShoot()
    {
        if (shootTime < shootCoolDown)
            shootTime += Time.deltaTime;
    }

    //public void ResetRobotHand()
    //{
    //    if (currentExistsTime >= RobotHandExistsTime && Shooted)
    //    {
    //        RobotHand.SetActive(false);
    //        Shooted = false;
    //        currentExistsTime = 0;
    //    }
    //    else if (Shooted)
    //        currentExistsTime += Time.deltaTime;
    //}
    public void ResetHand()
    {
        RobotHand.SetActive(false);
    }
}
