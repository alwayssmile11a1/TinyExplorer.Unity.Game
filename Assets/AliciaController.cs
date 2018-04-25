using BTAI;
using Gamekit2D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AliciaController : MonoBehaviour {
    public Transform targetToTrack;
    public float distance;

    //public Vector2 velocity;
    [Header("Attack1")]
    public GameObject bulletAttack1;
    public Transform[] shootRight;
    public Transform[] shootLeft;

    [Header("Attack2")]
    public float dashSpeed;
    public Damager attack2Damager;

    [Header("Attack3")]
    public Transform backwardPos;
    public GameObject Bomb;
    public Transform startRandomBombPos;
    public Transform endRandomBombPos;
    public int numberOfSoldier;

    private int explodingHash;

    private Animator animator;
    private new Rigidbody2D rigidbody2D;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider2D;
    
    /// <summary>
    /// Bullet of Attack1
    /// </summary>
    BulletPool bulletPool1;
    /// <summary>
    /// Bullet of Attack3
    /// </summary>
    BulletPool bulletPool3;
    BulletObject[] attack3BulletObject;

    Root AliciaBT = BT.Root();
    // Use this for initialization
    void Start() {
        bulletPool1 = BulletPool.GetObjectPool(bulletAttack1, 5);
        bulletPool3 = BulletPool.GetObjectPool(Bomb, 7);
        explodingHash = VFXController.StringToHash("ExplodingHitEffect");

        animator = GetComponentInChildren<Animator>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        boxCollider2D = GetComponent<BoxCollider2D>();

        AliciaBT.OpenBranch(
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
            ),
            BT.Sequence().OpenBranch(
                BT.Wait(2f),
                BT.SetBool(animator, "backward", true),
                BT.WaitForAnimatorState(animator, "backward"),
                BT.Call(MoveBackWard),
                BT.WaitUntil(CheckMoveBackWard),
                BT.SetBool(animator, "attack3", true),
                BT.SetBool(animator, "backward", false),
                BT.WaitForAnimatorState(animator, "attack3"),
                BT.Call(Attack3),
                BT.WaitUntil(Attack3Check),
                BT.SetBool(animator, "attack3", false),
                BT.Call(() => rigidbody2D.position = new Vector2(rigidbody2D.position.x, rigidbody2D.position.y - 0.55f)),
                BT.Call(() => Debug.Log("after attack3"))
            )
        );
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

    private void MoveBackWard()
    {
        rigidbody2D.velocity = Vector2.right * dashSpeed;
    }
    private bool CheckMoveBackWard()
    {
        if ((transform.position - backwardPos.position).sqrMagnitude <= 0.5)
        {
            rigidbody2D.velocity = new Vector2(0, 0);
            return true;
        }
        return false;
    }
    public void SetAttack3PosForAnimation()
    {
        rigidbody2D.position = new Vector2(rigidbody2D.position.x, rigidbody2D.position.y + 0.55f);
    }
    public void SetAttack3OldPosForAnimation()
    {
        rigidbody2D.position = new Vector2(rigidbody2D.position.x, rigidbody2D.position.y - 0.55f);
    }
    private void Attack3()
    {
        attack3BulletObject = new BulletObject[numberOfSoldier];
        for (int i = 0; i < numberOfSoldier; ++i)
        {
            float x = Random.Range(startRandomBombPos.position.x, endRandomBombPos.position.x);
            attack3BulletObject[i] = bulletPool3.Pop(new Vector2(x, startRandomBombPos.position.y));
            attack3BulletObject[i].instance.GetComponent<MiniSoldier>().Active();
        }
    }
    private bool Attack3Check()
    {
        foreach (var item in attack3BulletObject)
        {
            if (!item.inPool)
                return false;
        }
        return true;
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
