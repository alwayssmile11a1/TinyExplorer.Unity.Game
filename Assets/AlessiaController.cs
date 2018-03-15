using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController2D))]
public class AlessiaController : MonoBehaviour {

    public float speed = 5f;
    public float jumpForce = 400f;

    public FireFlyController fireFlyController;

    
    public KeyCode jumpButton;
    public KeyCode shootButton;

    private CharacterController2D m_CharacterController2D;
    private Vector2 m_Velocity = new Vector2();
    private Rigidbody2D m_Rigidbody2D;
    private float m_HorizontalMovement;
    private float m_VerticalForce;
    private Animator m_Animator;
    private SpriteRenderer m_SpriteRenderer;


    void Awake () {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_Animator = GetComponent<Animator>();
        m_CharacterController2D = GetComponent<CharacterController2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //Get horizontal move
        m_HorizontalMovement = Input.GetAxis("Horizontal");

        //jump only if grounded
        if (Input.GetKeyDown(jumpButton) && m_CharacterController2D.IsGrounded)
        {

            m_VerticalForce = jumpForce;
        }
        
        //shoot
        if(Input.GetKeyDown(shootButton))
        {
            Shoot();
        }
        
        //set velocity 
        m_Velocity.Set(m_HorizontalMovement * speed, m_Rigidbody2D.velocity.y);

    }


    private void FixedUpdate()
    {

        Move();
        Face();
        Animate();
    }

    private void Move()
    {
        m_Rigidbody2D.velocity = m_Velocity;
        m_Rigidbody2D.AddForce(new Vector2(0, m_VerticalForce));
        m_VerticalForce = 0;
    }
    
    private void Face()
    {
        if(!m_SpriteRenderer.flipX && m_HorizontalMovement<0)
        {
            m_SpriteRenderer.flipX = true;
        }

        if (m_SpriteRenderer.flipX && m_HorizontalMovement > 0)
        {
            m_SpriteRenderer.flipX = false;
        }

    }

    private void Animate()
    {
        m_Animator.SetBool("grounded", m_CharacterController2D.IsGrounded);
        m_Animator.SetFloat("velocityX", Mathf.Abs(m_HorizontalMovement));
    }

    private void Shoot()
    {
        if(fireFlyController!=null)
        {
           fireFlyController.Trigger();
        }
    }



}
