using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidBubblesController : MonoBehaviour {

    public float bulletSpeed = 5f;

    private Rigidbody2D m_Rigidbody2D;



    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
    }


    void FixedUpdate () {
        m_Rigidbody2D.velocity = new Vector2(transform.right.x * bulletSpeed, 0);
      
	}

}
