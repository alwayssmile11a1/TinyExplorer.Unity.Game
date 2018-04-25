using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartShooting : MonoBehaviour {
    public Vector2 direction;
    public float speed;
    private new Rigidbody2D rigidbody2D;


    // Use this for initialization
    void Awake () {
        rigidbody2D = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
        rigidbody2D.velocity = direction * speed;
	}

    public void ReverseVelocity()
    {
        direction *= -1;
    }
}
