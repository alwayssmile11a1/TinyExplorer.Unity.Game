using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallTowardTarget : MonoBehaviour {
    public Transform target;
    public Vector2 velocity;
    private Vector2 negativeXVelocity;
    private SpriteRenderer targetSpriteRenderer;
    private new Rigidbody2D rigidbody2D;
	// Use this for initialization
	void Awake () {
        rigidbody2D = GetComponent<Rigidbody2D>();
        negativeXVelocity = new Vector2(-velocity.x, velocity.y);
	}

    // Update is called once per frame
    void Update()
    {
        if(transform.position.x < target.position.x)
        {
            rigidbody2D.velocity = velocity;
        }
        else
        {
            rigidbody2D.velocity = negativeXVelocity;
        }
    }

    public void SetVelocity()
    {
        rigidbody2D.velocity = velocity;
    }
}
