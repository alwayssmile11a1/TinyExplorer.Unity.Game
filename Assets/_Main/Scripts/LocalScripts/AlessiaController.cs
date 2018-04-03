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

    [Tooltip("Throw speed when get hit")]
    public Vector2 throwSpeed = new Vector2(3,3);


    private CharacterController2D m_CharacterController2D;
    private Vector2 m_Velocity = new Vector2();
    private Rigidbody2D m_Rigidbody2D;
    private Vector2 m_ForceVector;
    private Vector2 m_ThrowVector;

    private Animator m_Animator;
    private SpriteRenderer m_SpriteRenderer;
    private CharacterInput m_CharacterInput;
    private Flicker m_Flicker;

    private int m_HashGroundedPara = Animator.StringToHash("Grounded");
    private int m_HashRunPara = Animator.StringToHash("Run");
    private int m_HashHurtPara = Animator.StringToHash("Hurt");

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
            m_ForceVector.y = jumpForce;
        }
    }

    public void GotHit(Damager damager, Damageable damageable)
    {
        //throw player away a little bit
        m_ThrowVector = new Vector2(0, throwSpeed.y);
        Vector2 damagerToThis = damager.transform.position - transform.position;
        m_ThrowVector.x = Mathf.Sign(damagerToThis.x) * -throwSpeed.x;

        //Set animation
        m_Animator.SetTrigger(m_HashHurtPara);

        //Flicker
        m_Flicker.StartFlickering(damageable.invulnerabilityDuration, timeBetweenFlickering);

        //Shake camera a little
        CameraShaker.Shake(0.1f, 0.1f);

    }




    private void FixedUpdate()
    {
        //set velocity 
        m_Velocity.Set(m_CharacterInput.HorizontalAxis * speed, m_Rigidbody2D.velocity.y);
        m_Velocity += m_ThrowVector;

        Move();
        Face();
        Animate();
    }

    private void Move()
    {
        m_Rigidbody2D.velocity = m_Velocity;
        m_Rigidbody2D.AddForce(m_ForceVector, ForceMode2D.Impulse);

        m_ThrowVector.x = m_ThrowVector.x != 0 ? m_ThrowVector.x - Mathf.Sign(m_ThrowVector.x)*0.1f : 0;
        m_ThrowVector.y = 0;
        m_ForceVector = Vector2.zero;
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
