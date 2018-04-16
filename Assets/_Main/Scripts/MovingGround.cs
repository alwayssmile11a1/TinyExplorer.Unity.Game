using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovingGround : MonoBehaviour {
    public Transform destination;
    public Vector3 velocity;
    [SerializeField]
    private bool canMove;
    private Rigidbody2D platformRigidbody;
    //[SerializeField]
    private Vector3 m_des;
    public GameObject Effect;
    private void Awake()
    {
        platformRigidbody = GetComponent<Rigidbody2D>();
        m_des = new Vector3(destination.position.x, destination.position.y, 0);
    }
    // Update is called once per frame
    void Update () {
        if(canMove)
        {
            Vector3 movement = velocity * Time.deltaTime;
            MovePlatform(movement);
        }
	}

    void MovePlatform(Vector3 movement)
    {
        float dist = Vector3.Distance(m_des, transform.position);
        Debug.Log("Distance to other: " + dist);
        Debug.Log("m_des: " + m_des);
        Debug.Log("m_transform: " + transform.position);
        if (dist > 0.05)
        {
            //Debug.Log(destination);
            Vector2 currentPosition = platformRigidbody.position;
            //platformRigidbody.MovePosition(currentPosition + movement);
            platformRigidbody.transform.position += movement;
        }
        else
            canMove = false;

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player")
        {
            Debug.Log("set in ground");
            collision.collider.transform.SetParent(transform);
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player")
        {
            Debug.Log("unset in ground");
            collision.collider.transform.SetParent(null);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Player"))
        {
            canMove = true;
            var children = Effect.GetComponentsInChildren<ParticleSystem>();
            foreach(var child in children)
            {
                if (child.tag == "ParticleSystem")
                {
                    child.Stop();
                }
            }
            //triggerEffect.Stop();
        }
    }
}
