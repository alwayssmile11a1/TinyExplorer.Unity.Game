using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovingGround : MonoBehaviour {
    public Transform destination;
    public Vector2 velocity;
    [SerializeField]
    private bool canMove;
    private Rigidbody2D platformRigidbody;

    private void Awake()
    {
        platformRigidbody = GetComponent<Rigidbody2D>();
    }
    // Update is called once per frame
    void Update () {
        if(canMove)
        {
            Vector2 movement = velocity * Time.deltaTime;
            MovePlatform(movement);
        }
	}

    void MovePlatform(Vector2 movement)
    {
        if (true)
        {
            Vector2 currentPosition = platformRigidbody.position;
            platformRigidbody.MovePosition(currentPosition + movement);
        }
        //else
        //    canMove = false;
    }
}
