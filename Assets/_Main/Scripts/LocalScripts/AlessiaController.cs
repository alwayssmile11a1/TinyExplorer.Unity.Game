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
    public Transform shieldPositon;
    [SerializeField]
    private Object Shield;

    [Tooltip("Throw speed when get hit")]
    public Vector2 throwSpeed = new Vector2(3,3);

    [Header("Dash")]
    public GameObject dashEffect;
    public float dashSpeed = 5f;
    public float dashDuration = 1f;
    public float dashCooldDownTime = 1f;

    private CharacterController2D m_CharacterController2D;
    private Vector2 m_Velocity = new Vector2();
    private Rigidbody2D m_Rigidbody2D;
    private Vector2 m_JumpForceVector;
    private Vector2 m_ThrowVector;
    //private float disableMoveTimeAfterHit;

    private Animator m_Animator;
    private SpriteRenderer m_SpriteRenderer;
    private CharacterInput m_CharacterInput;
    private Flicker m_Flicker;

    private int m_HashGroundedPara = Animator.StringToHash("Grounded");
    private int m_HashRunPara = Animator.StringToHash("Run");
    private int m_HashHurtPara = Animator.StringToHash("Hurt");

    private bool m_Dashing;
    private float m_DashTimer;
    private bool m_CanDash = true;
    

    private void Awake () {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_Animator = GetComponent<Animator>();
        m_CharacterController2D = GetComponent<CharacterController2D>();
        m_CharacterInput = GetComponent<CharacterInput>();
        m_Flicker = gameObject.AddComponent<Flicker>();

    }

    private void Update()
    {
        if(m_DashTimer>0)
        {
            m_DashTimer -= Time.deltaTime;

            if (m_DashTimer <= 0)
            {
                m_CanDash = true;
            }

        }
    }

    public void Jump()
    {
        if (m_CharacterController2D.IsGrounded)
        {
            m_JumpForceVector.y = jumpForce;
        }
    }

    public void SpawnShield()
    {
        Debug.Log("spawn shield");
        Object shied = Instantiate(Shield, shieldPositon);
        Destroy(shied, 0.5f);
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

    public void Dash()
    {
        if (!m_CanDash) return;
        StartCoroutine(InternalDash());
    }

    private IEnumerator InternalDash()
    {
        //enable dash effect
        float originalGravity = m_Rigidbody2D.gravityScale;
        m_Rigidbody2D.gravityScale = 0;
        dashEffect.SetActive(true);
        m_Dashing = true;
        m_CanDash = false;

        //get direction
        Vector2 direction = m_SpriteRenderer.flipX ? Vector2.left : Vector2.right;

        //dash
        m_Rigidbody2D.velocity = direction * dashSpeed;


        yield return new WaitForSeconds(dashDuration);

        //disable dash effect 
        m_Rigidbody2D.gravityScale = originalGravity;
        dashEffect.SetActive(false);
        m_Dashing = false;
        m_DashTimer = dashCooldDownTime;
    }

    private void FixedUpdate()
    {
        if (m_Dashing) return;

        Move();
        Face();
        Animate();
    }

    private void Move()
    {
       
        //set velocity 
        m_Velocity.Set(m_CharacterInput.HorizontalAxis * speed + m_ThrowVector.x, m_ThrowVector.y > 0 ? m_ThrowVector.y : m_Rigidbody2D.velocity.y);

        //Move rigidbody
        m_Rigidbody2D.velocity = m_Velocity;
        m_Rigidbody2D.AddForce(m_JumpForceVector, ForceMode2D.Impulse);

        m_ThrowVector.x = Mathf.Abs(m_ThrowVector.x) > 0.2f ? (m_ThrowVector.x - Mathf.Sign(m_ThrowVector.x) * 0.1f) : 0;

        m_ThrowVector.y = 0;
        m_JumpForceVector = Vector2.zero;
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
