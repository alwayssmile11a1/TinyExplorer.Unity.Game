using BTAI;
using Gamekit2D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AliciaController : MonoBehaviour {
    public Transform targetToTrack;

    private float currentTimeInIdle;

   
    public float distance;

    //public Vector2 velocity;
    [Header("Attack1")]
    public GameObject bulletAttack1;
    public Transform[] shootRight;
    public Transform[] shootLeft;

    [Header("Attack2")]
    public float dashSpeed;
    public Damager attack2Damager;

    private int explodingHash;

    private Animator animator;
    private new Rigidbody2D rigidbody2D;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider2D;
    Vector2 endPos;

    BulletPool bulletPool1;

    Root AliciaBT = BT.Root();
    // Use this for initialization
    void Start() {
        currentTimeInIdle = 0;
        bulletPool1 = BulletPool.GetObjectPool(bulletAttack1, 5);
        explodingHash = VFXController.StringToHash("ExplodingHitEffect");

        animator = GetComponentInChildren<Animator>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        boxCollider2D = GetComponent<BoxCollider2D>();

        AliciaBT.OpenBranch(
            BT.If(() => { return true; }).OpenBranch(
                BT.Sequence().OpenBranch(
                    BT.Wait(3),
                    BT.Call(OrientToTarget),
                    BT.SetBool(animator, "attack1", true),
                    BT.WaitForAnimatorState(animator, "attack1"),
                    BT.SetBool(animator, "attack1", false),
                    BT.Call(Attack1),
                    BT.WaitForAnimatorState(animator, "Idle"),
                    BT.Call(() => Debug.Log("Finish Dash"))
                ),
                BT.Sequence().OpenBranch(
                    BT.Wait(2f),
                    BT.Call(OrientToTarget),
                    BT.SetBool(animator, "dash", true),
                    BT.WaitForAnimatorState(animator, "beforeDash"),
                    BT.WaitForAnimatorState(animator, "dash"),
                    BT.Call(MoveToTarget),
                    BT.WaitUntil(MoveCheck),
                    BT.SetBool(animator, "dash", false)
                ),
                BT.Sequence().OpenBranch(
                    BT.Call(OrientToTarget),
                    BT.SetBool(animator, "attack2", true),
                    BT.WaitForAnimatorState(animator, "attack2"),
                    BT.SetBool(animator, "attack2", false),
                    BT.Call(Attack2)
                )
            )
        );
        endPos = (transform.position + targetToTrack.position) * 0.5f;
    }
	
	// Update is called once per frame
	void Update () {
        AliciaBT.Tick();
	}

    //private void ScanForTarget()
    //{
    //    Vector2 origin = rigidbody2D.position + boxCollider2D.offset;
    //    RaycastHit2D raycastHit2DRight = Physics2D.Raycast(origin, Vector2.right, distance, LayerMask.GetMask("Player"));
    //    RaycastHit2D raycastHit2DLeft = Physics2D.Raycast(origin, Vector2.left, distance, LayerMask.GetMask("Player"));
    //    Debug.DrawRay(transform.position, Vector2.right * distance);
    //    Debug.DrawRay(transform.position, Vector2.left * distance);
    //    if (raycastHit2DRight && raycastHit2DRight.collider.tag.Equals("Player"))
    //    {
    //        Debug.Log("right: " + raycastHit2DRight.collider.tag);
    //    }
    //    if(raycastHit2DLeft && raycastHit2DLeft.collider.tag.Equals("Player"))
    //    {
    //        Debug.Log("left: " + raycastHit2DLeft.collider.tag);
    //    }
    //}

    private void Attack1()
    {
        Debug.Log("Attack1");
        if(spriteRenderer.flipX)
        {
            foreach (var item in shootRight)
            {
                Debug.Log("Pop bullet");
                BulletObject bullet = bulletPool1.Pop(item.position);
                bullet.instance.GetComponent<StartShooting>().direction = Vector2.right;
            }
        }
        else
        {
            foreach (var item in shootLeft)
            {
                BulletObject bullet = bulletPool1.Pop(item.position);
                bullet.instance.GetComponent<StartShooting>().direction = Vector2.left;
            }
        }
    }
    private void MoveToTarget()
    {
        Debug.Log("Move");
        Vector2 direction = (targetToTrack.position - transform.position).normalized;

        rigidbody2D.velocity = new Vector2(direction.x, 0) * dashSpeed;
    }
    private bool MoveCheck()
    {
        OrientToTarget();
        if(transform.position.x < targetToTrack.position.x && rigidbody2D.velocity.x < 0)
        {
            rigidbody2D.velocity = -rigidbody2D.velocity;
        }
        else if(transform.position.x >= targetToTrack.position.x && rigidbody2D.velocity.x > 0)
        {
            rigidbody2D.velocity = -rigidbody2D.velocity;
        }
        if ((transform.position - targetToTrack.position).sqrMagnitude <= 1.3f)
        {
            rigidbody2D.velocity = Vector2.zero;
            return true;
        }
        return false;
    }
    private void OrientToTarget()
    {
        if (targetToTrack == null) return;

        if (targetToTrack.position.x > transform.position.x && !spriteRenderer.flipX)
        {
            FlipAlicia(true);
        }
        else if (targetToTrack.position.x <= transform.position.x && spriteRenderer.flipX)
        {
            FlipAlicia(false);
        }
    }

    private void FlipAlicia(bool b)
    {
        spriteRenderer.flipX = b;
        boxCollider2D.offset = -boxCollider2D.offset;
        attack2Damager.offset = -attack2Damager.offset;
    }

    private void Attack2()
    {
        Debug.Log("Attack2");
    }

    public void EnableDamager()
    {
        attack2Damager.EnableDamage();
        Debug.Log("Enabled");
    }

    public void DisableDamager()
    {
        attack2Damager.DisableDamage();
        Debug.Log("Disabled");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("OnTriggerEnter: " + collision.tag);
    }
    public void OnDie()
    {
        BlackKnightController.aliciaDied = true;
    }
}
