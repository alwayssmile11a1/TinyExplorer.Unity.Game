using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingGroundDownLoop : MonoBehaviour {
    public EdgeCollider2D edge;
    public Vector2 velocity;

    private Rigidbody2D rigidbody2D;
    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        rigidbody2D.position += velocity * Time.deltaTime;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player")
            collision.collider.transform.SetParent(transform);
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player")
            collision.collider.transform.SetParent(null);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("return");
    }
}
