using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rmx6Moving : MonoBehaviour {
    public float timeToTurn;
    [SerializeField]
    private float currentTimeMove;
    public Vector2 velocity;

    [SerializeField]
    public bool canWalk;
    private new Rigidbody2D rigidbody2D;
    private SpriteRenderer spriteRenderer;
    private CapsuleCollider2D capsuleJumpRange;
    // Use this for initialization
    void Start () {
        currentTimeMove = 0;
        rigidbody2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        capsuleJumpRange = GetComponentInChildren<CapsuleCollider2D>();
    }
	
	// Update is called once per frame
	void Update () {
        if(canWalk)
        {
            rmx6Move();
        }
	}

    private void rmx6Move()
    {
        //Debug.Log("velocity in moveing: " + velocity);
        Vector2 movement = velocity * Time.deltaTime;
        rigidbody2D.position += movement;
        currentTimeMove += Time.deltaTime;
        if (currentTimeMove >= timeToTurn)
        {
            currentTimeMove = 0;
            spriteRenderer.flipX = !spriteRenderer.flipX;
            capsuleJumpRange.offset = new Vector2(capsuleJumpRange.offset.x * -1, capsuleJumpRange.offset.y);
            velocity = new Vector2(velocity.x * -1, velocity.y);
        }
    }
}
