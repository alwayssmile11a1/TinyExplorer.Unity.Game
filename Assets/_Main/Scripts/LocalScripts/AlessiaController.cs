using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamekit2D;

[RequireComponent(typeof(CharacterInput))]
[RequireComponent(typeof(CharacterController2D))]
public class AlessiaController : MonoBehaviour {

    public float speed = 5f;
    public float jumpForce = 400f;
    public float timeBetweenFlickering = 0;

    private CharacterController2D m_CharacterController2D;
    private Vector2 m_Velocity = new Vector2();
    private Rigidbody2D m_Rigidbody2D;
    private float m_VerticalForce;
    private Animator m_Animator;
    private SpriteRenderer m_SpriteRenderer;
    private CharacterInput m_CharacterInput;
    private Flicker m_Flicker;

    private int m_HashGroundedPara = Animator.StringToHash("Grounded");
    private int m_HashRunPara = Animator.StringToHash("Run");


    void Awake () {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_Animator = GetComponent<Animator>();
        m_CharacterController2D = GetComponent<CharacterController2D>();
        m_CharacterInput = GetComponent<CharacterInput>();
        m_Flicker = gameObject.AddComponent<Flicker>();

    }

    private void Start()
    {
        //flicker = new Flicker();
        
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

    public void GotHit(Damager damager, Damageable damageable)
    {
        m_Flicker.StartFlickering(damageable.invulnerabilityDuration, timeBetweenFlickering);

        CameraShaker.Shake(0.1f, 0.1f);

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
