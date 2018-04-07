using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RideloidShoot : MonoBehaviour {
    public LayerMask hitLayerMask;
    public float castDistance;
    public GameObject RobotHand; 
    private Vector2 direction;
    private ContactFilter2D contactFilter2D;
    private CapsuleCollider2D capsuleCollider2D;
    private Rigidbody2D rigidbody2D;
    private Animator animator;
    private bool foundPlayer;
    private List<GameObject> RobotHands;
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
        foundPlayer = false;
        direction = Vector2.right;
        contactFilter2D.layerMask = hitLayerMask;
        animator = GetComponent<Animator>();
        capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        Offset = new Vector2(capsuleCollider2D.size.x, 0);
	}

    private void Start()
    {
        // initialize RobotHands
    }

    private void FixedUpdate()
    {
        ScanForPlayer();
    }

    void ScanForPlayer()
    {
        RaycastHit2D[] hit = new RaycastHit2D[5];
        Vector2 Origin = rigidbody2D.position + Offset;
        Physics2D.Raycast(Origin, direction, contactFilter2D, hit, castDistance);
        Debug.Log(hit);
        Debug.DrawRay(Origin , direction * castDistance);
        foreach (RaycastHit2D item in hit)
        {
            if (item.rigidbody != null && item.collider.tag.Equals("Player"))
            {
                foundPlayer = true;
                ShootPlayer();
            }
        }
        if (!foundPlayer && animator.GetBool("attack"))
            animator.SetBool("attack", false);
        foundPlayer = false;
    }

    void ShootPlayer()
    {
        Debug.Log("Shoot");
        animator.SetBool("attack", true);
    }

    public void Shooting()
    {
        RobotHand.SetActive(true);
    }
}
