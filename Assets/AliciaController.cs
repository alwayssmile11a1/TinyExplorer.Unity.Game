using BTAI;
using Gamekit2D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AliciaController : MonoBehaviour {
    public Transform targetToTrack;

    private float currentTimeInIdle;

   
    public float distance;
    public bool isReplica;
    //public Vector2 velocity;
    [Header("Attack1")]
    public GameObject bulletAttack1;
    public Transform shootRight;
    public Transform shootLeft;

    [Header("Attack3")]
    public Transform newPos;
    public Transform oldPos;
    public float speed;
    public GameObject aliciaReplica;
    public GameObject spawnReplica;


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

        animator = GetComponentInChildren<Animator>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        boxCollider2D = GetComponent<BoxCollider2D>();

        AliciaBT.OpenBranch(
            BT.If(() => { return true; }).OpenBranch(
                BT.Sequence().OpenBranch(
                    BT.Wait(3),
                    BT.Call(() => animator.SetBool("attack1", true)),
                    BT.WaitForAnimatorState(animator, "attack1"),
                    BT.Call(() => animator.SetBool("attack1", false)),
                    BT.Call(Attack1),
                    BT.WaitForAnimatorState(animator, "Idle"),
                    BT.Call(() => Debug.Log("Finish Dash"))
                ),
                BT.Sequence().OpenBranch(
                    BT.Wait(2f),
                    BT.Call(() => animator.SetBool("attack2", true)),
                    BT.WaitForAnimatorState(animator, "attack2"),
                    BT.Call(() => animator.SetBool("attack2", false)),
                    BT.Call(Attack2)
                ),
                BT.If(() => !isReplica).OpenBranch(
                    BT.Sequence().OpenBranch(
                    BT.Wait(2f),
                    BT.Call(MoveToOldPos),
                    BT.WaitUntil(MoveCheck),
                    BT.Wait(1.5f),
                    BT.Call(ActiveReplica),
                    BT.Wait(20f)
                    // nếu giết đc phân thân thì sao đó
                    )
                )
            )
        );
        endPos = (transform.position + targetToTrack.position) * 0.5f;
    }
	
	// Update is called once per frame
	void Update () {
        AliciaBT.Tick();
	}

    private void ScanForTarget()
    {
        Vector2 origin = rigidbody2D.position + boxCollider2D.offset;
        RaycastHit2D raycastHit2DRight = Physics2D.Raycast(origin, Vector2.right, distance, LayerMask.GetMask("Player"));
        RaycastHit2D raycastHit2DLeft = Physics2D.Raycast(origin, Vector2.left, distance, LayerMask.GetMask("Player"));
        Debug.DrawRay(transform.position, Vector2.right * distance);
        Debug.DrawRay(transform.position, Vector2.left * distance);
        if (raycastHit2DRight && raycastHit2DRight.collider.tag.Equals("Player"))
        {
            Debug.Log("right: " + raycastHit2DRight.collider.tag);
        }
        if(raycastHit2DLeft && raycastHit2DLeft.collider.tag.Equals("Player"))
        {
            Debug.Log("left: " + raycastHit2DLeft.collider.tag);
        }
    }

    private void Attack1()
    {
        Debug.Log("Attack1");
        if(spriteRenderer.flipX)
        {
            Debug.Log("Pop bullet");
            BulletObject bullet = bulletPool1.Pop(shootRight.position);
            bullet.instance.GetComponent<StartShooting>().direction = Vector2.right;
        }
        else
        {
            BulletObject bullet = bulletPool1.Pop(shootLeft.position);
            bullet.instance.GetComponent<StartShooting>().direction = Vector2.left;
        }
    }

    private void MoveToOldPos()
    {
        Debug.Log("Move");
        Vector2 direction = (newPos.position - transform.position).normalized;

        rigidbody2D.velocity = direction * speed;
    }
    private bool MoveCheck()
    {
        if ((transform.position - newPos.position).sqrMagnitude <= 0.5f)
        {
            rigidbody2D.velocity = Vector2.zero;
            return true;
        }

        return false;
    }
    private void ActiveReplica()
    {
        Debug.Log("Spawn replica");
        aliciaReplica.SetActive(true);
        spawnReplica.SetActive(true);
    }

    private void Attack2()
    {
        Debug.Log("Attack2");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("OnTriggerEnter: " + collision.tag);
    }
}
