using Gamekit2D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovingGround : MonoBehaviour {
    enum MoveType
    {
        OneWay,
        Infinite
    }
    public Transform destination;
    [SerializeField]
    private MoveType moveType = MoveType.OneWay;
    public Vector3 velocity;
    [SerializeField]
    private bool canMove;
    private Rigidbody2D platformRigidbody;
    [SerializeField]
    private float distance;
    private Vector3 m_des;
    private Vector3 m_des2;
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
        m_des = destination.position;
        m_des2 = transform.position;
        if(m_des.x == 221)
        {
            Vector3 temp = (m_des - transform.position).normalized;
            Debug.Log(m_des + " " + temp);
        }
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
        //Debug.Log("Distance to other: " + moveType + " " + dist);
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
            if(moveType.Equals(MoveType.OneWay))
                CanMove = false;
            else if(moveType.Equals(MoveType.Infinite))
            {
                SwapDestination(ref m_des, ref m_des2);
                velocity *= -1;
            }
        }
    }
    void SwapDestination(ref Vector3 a, ref Vector3 b)
    {
        Vector3 temp = a;
        a = b;
        b = temp;
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player" && velocity.y == 0 && canMove)
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
            if (Effect)
            {
                var children = Effect.GetComponentsInChildren<ParticleSystem>();
                foreach (var child in children)
                {
                    if (child.tag == "ParticleSystem")
                    {
                        child.Stop();
                    }
                }
            }
            //triggerEffect.Stop();
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Display the explosion radius when selected
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(m_des, 0.1f);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(m_des2, 0.1f);
    }
}
