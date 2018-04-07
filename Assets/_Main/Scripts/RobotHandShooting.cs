using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotHandShooting : MonoBehaviour {
    public Vector2 velocity;
    public Transform shootOrigin;
    private Rigidbody2D rigidbody2D;
	// Use this for initialization
	void Start () {
        velocity = new Vector2(1, 0);
        rigidbody2D = GetComponent<Rigidbody2D>();
        rigidbody2D.position = shootOrigin.position;
	}
	
	// Update is called once per frame
	void Update () {
        rigidbody2D.position += velocity * Time.deltaTime;
	}
}
