using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterInput))]
[RequireComponent(typeof(CharacterController2D))]
public class AlessiaController : MonoBehaviour {

    public float speed = 5f;
    public float jumpForce = 400f;

    private CharacterController2D m_CharacterController2D;
    private Vector2 m_Velocity = new Vector2();
    private Rigidbody2D m_Rigidbody2D;
    private float m_VerticalForce;
    private Animator m_Animator;
    private SpriteRenderer m_SpriteRenderer;
    private CharacterInput m_CharacterInput;

    private int m_HashGroundedPara = Animator.StringToHash("Grounded");
    private int m_HashRunPara = Animator.StringToHash("Run");

    void Awake () {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_Animator = GetComponent<Animator>();
        m_CharacterController2D = GetComponent<CharacterController2D>();
        m_CharacterInput = GetComponent<CharacterInput>();
    }

    // Update is called once per frame
    void Update()
    {
       

    }

    public void Jump()
    {
        if (m_CharacterController2D.IsGrounded)
        {
            m_VerticalForce = jumpForce;
        }
    }

    private void FixedUpdate()
    {
        //set velocity 
        m_Velocity.Set(m_CharacterInput.HorizontalAxis * speed, m_Rigidbody2D.velocity.y);

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
        if(!m_SpriteRenderer.flipX && m_CharacterInput.HorizontalAxis < 0)
        {
            m_SpriteRenderer.flipX = true;
        }

        if (m_SpriteRenderer.flipX && m_CharacterInput.HorizontalAxis > 0)
        {
            m_SpriteRenderer.flipX = false;
        }

    }

    private void Animate()
    {
        m_Animator.SetBool(m_HashGroundedPara, m_CharacterController2D.IsGrounded);
        m_Animator.SetFloat(m_HashRunPara, Mathf.Abs(m_CharacterInput.HorizontalAxis));
    }

}
