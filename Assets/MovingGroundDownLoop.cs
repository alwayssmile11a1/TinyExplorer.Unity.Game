using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingGroundDownLoop : MonoBehaviour {
    public Transform pos;
    public Vector3 velocity;
    private Transform t;
    private new Rigidbody2D rigidbody2D;
    
    private void Awake()
    {
        t = GetComponent<Transform>();
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        rigidbody2D.transform.position += velocity * Time.deltaTime;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player")
        {
            Debug.Log("set");
            collision.collider.transform.SetParent(transform);
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player")
        {
            Debug.Log("unset");
            collision.collider.transform.SetParent(null);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("reach edge");
        if (collision.tag.Equals("Finish"))
        {
            transform.position = pos.position;
            Debug.Log("this transform " + transform.position);
            Debug.Log("pos transform " + pos.position);
        }
    }
}
