using Gamekit2D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingInRange : MonoBehaviour {
    public Transform[] Range;
    public Vector2 velocity;
    private Rigidbody2D rideloidgRigidbody;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private bool canMove;
    private int normalState, walkingState;
    [SerializeField][Range(5, 15)]
    private int numberOfNormalState, numberOfWalkingState;
    [SerializeField][Range(0.5f, 1)]
    private float timeWaitForAnimation;
    private RideloidShoot rideloidShoot;
    private Damageable damageable;
    // Use this for initialization
    void Awake()
    {
        walkingState = 0;
        normalState = 0;
        canMove = false;
        foreach (var r in Range) r.SetParent(null);
        rideloidgRigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        animator.SetBool("isInNormalState", true);
        rideloidShoot = GetComponent<RideloidShoot>();
        damageable = GetComponent<Damageable>();
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove && !animator.GetBool("attack"))
        {
            MoveRideloidg();
         
        }
    }

    private void MoveRideloidg()
    {
        Vector2 movement = velocity * Time.deltaTime;
        rideloidgRigidbody.position += movement;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag.Equals("RideloidRange"))
        {
            FlipRideloid();
        }
    }

    public void FlipRideloid()
    {
        velocity *= -1;
        spriteRenderer.flipX = !spriteRenderer.flipX;
        rideloidShoot.Direction *= -1;
        rideloidShoot.Offset *= -1;
    }

    public void countNormalState()
    {
        normalState++;
        if (normalState == numberOfNormalState)
        {
            normalState = 0;
            animator.SetBool("walk", true);
            animator.SetBool("isInNormalState", false);
            animator.SetBool("isInWalkingState", true);
            StartCoroutine(waitForAnimation(timeWaitForAnimation));
        }
    }
    public void countWalkingState()
    {
        walkingState++;
        if (walkingState == numberOfWalkingState)
        {
            walkingState = 0;
            animator.SetBool("walk", false);
            animator.SetBool("isInNormalState", true);
            animator.SetBool("isInWalkingState", false);
            StartCoroutine(waitForAnimation(timeWaitForAnimation));
        }
    }

    IEnumerator waitForAnimation(float time)
    {
        yield return new WaitForSeconds(time);
        canMove = !canMove;
    }
    private void OnDisable()
    {
        walkingState = 0;
        normalState = 0;
        canMove = false;
        //damageable.SetHealth(damageable.startingHealth);
    }
    private void OnEnable()
    {
        animator.SetBool("isInNormalState", true);
    }
}
