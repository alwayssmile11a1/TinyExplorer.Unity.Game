using Gamekit2D;
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
    [SerializeField]
    private float distance;
    private Vector3 m_des;
    public GameObject Effect;

    public bool CanMove
    {
        get
        {
            return canMove;
        }

        set
        {
            canMove = value;
        }
    }

    private void Awake()
    {
        platformRigidbody = GetComponent<Rigidbody2D>();
        m_des = new Vector3(destination.position.x, destination.position.y, 0);
    }
    // Update is called once per frame
    void Update () {
        if(CanMove)
        {
            Vector3 movement = velocity * Time.deltaTime;
            MovePlatform(movement);
        }
	}

    void MovePlatform(Vector3 movement)
    {
        float dist = Vector3.Distance(m_des, transform.position);
        //Debug.Log("Distance to other: " + dist);
        //Debug.Log("m_des: " + m_des);
        //Debug.Log("m_transform: " + transform.position);
        //if (dist > distance)
        if (dist > distance)
        {
            //Debug.Log(destination);
            Vector2 currentPosition = platformRigidbody.position;
            //platformRigidbody.MovePosition(currentPosition + movement);
            platformRigidbody.transform.position += movement;
        }
        else
        {
            CanMove = false;
        }

    }
    void MovePlatform_1() { 
}
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player" && velocity.y == 0)
        {
            Debug.Log("set in ground");
            collision.gameObject.GetComponent<CharacterController2D>().Move(velocity*Time.deltaTime);
        }
    }
    //private void OnCollisionExit2D(Collision2D collision)
    //{
    //    if (collision.collider.tag == "Player")
    //    {
    //        Debug.Log("unset in ground");
    //        collision.collider.transform.SetParent(null);
    //    }
    //}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Player"))
        {
            CanMove = true;
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
