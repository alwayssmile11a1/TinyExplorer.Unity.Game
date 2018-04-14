using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotHandShooting : MonoBehaviour {
    [SerializeField]
    private Vector2 velocity;
    private Rigidbody2D rigidbody2D;
    // Use this for initialization
    void Awake () {
        rigidbody2D = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
        rigidbody2D.position += velocity * Time.deltaTime;
	}

    public void ReverseVelocity()
    {
        velocity *= -1;
    }
}
