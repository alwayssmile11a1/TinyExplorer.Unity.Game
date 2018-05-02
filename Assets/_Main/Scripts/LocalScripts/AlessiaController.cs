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
    public ParticleSystem shield;

    [Tooltip("Throw speed when get hit")]
    public Vector2 throwSpeed = new Vector2(3, 3);

    [Header("Dash")]
    public GameObject dashEffect;
    public float dashSpeed = 5f;
    public float dashDuration = 1f;
    public float dashCooldDownTime = 1f;

    [Header("Slash")]
    public ParticleSystem leftSlashEffect;
    public ParticleSystem rightSlashEffect;
    public Transform slashContactTransform;


    [Header("Audio")]
    public RandomAudioPlayer footStepAudioPlayer;
    public RandomAudioPlayer slashAudioPlayer;
    public RandomAudioPlayer landAudioPlayer;
    public RandomAudioPlayer dashAudioPlayer;
    public RandomAudioPlayer hurtAudioPlayer;

    private Damager m_Slash;
    private SimpleCharacterController2D m_CharacterController2D;
    private Vector2 m_Velocity = new Vector2();
    private Rigidbody2D m_Rigidbody2D;
    private Vector2 m_JumpForceVector;
    private Vector2 m_ThrowVector;
    
    private Animator m_Animator;
    private SpriteRenderer m_SpriteRenderer;
    private CharacterInput m_CharacterInput;
    private Flicker m_Flicker;

    private Transform m_AlessiaGraphics;
    private ParticleSystem m_SlashContactEffect;
    private Vector3 m_OffsetFromSlashEffectToAlessia;

    private int m_HashGroundedPara = Animator.StringToHash("Grounded");
    private int m_HashRunPara = Animator.StringToHash("Run");
    private int m_HashHurtPara = Animator.StringToHash("Hurt");
    private int m_HashDashPara = Animator.StringToHash("Dash");

    private bool m_BlockNormalAction;
    private float m_DashCoolDownTimer;
    private float m_DashTimer;
    private float m_OriginalGravity;
    private bool m_CanDash = true;
    private bool m_CanSlash = true;
    //Allow dash in air only one time
    private bool m_DashedInAir = false;

    private float m_AttackTimer;
    private float m_ExternalForceTimer;


    private Checkpoint m_LastCheckpoint = null;
    private Damageable m_Damageable;


    private void Awake () {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        m_SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        m_AlessiaGraphics = m_SpriteRenderer.gameObject.transform;
        m_Animator = GetComponent<Animator>();
        m_CharacterController2D = GetComponent<SimpleCharacterController2D>();
        m_CharacterInput = GetComponent<CharacterInput>();
        m_Flicker = m_SpriteRenderer.gameObject.AddComponent<Flicker>();
        m_Slash = GetComponent<Damager>();
        m_Slash.DisableDamage();
        m_SlashContactEffect = slashContactTransform.GetComponentInChildren<ParticleSystem>();
        m_OffsetFromSlashEffectToAlessia = slashContactTransform.position - transform.position;
        m_Damageable = GetComponent<Damageable>();
    }

    private void Update()
    {
        if(m_DashCoolDownTimer>0)
        {
            m_DashCoolDownTimer -= Time.deltaTime;

            if (m_DashCoolDownTimer <= 0)
            {
                //if we haven't grounded yet, can not dash more (allow dash only 1 time in air)
                if (m_DashedInAir && !m_CharacterController2D.IsGrounded)
                {
                    m_DashCoolDownTimer += Time.deltaTime;
                }
                else
                {
                    m_DashedInAir = false;
                    m_CanDash = true;
                }
            }
        }

        if (m_DashTimer > 0)
        {
            m_DashTimer -= Time.deltaTime;

            if (m_DashTimer <= 0)
            {
                EndDashing();
            }
        }

        if (m_AttackTimer > 0)
        {
            m_AttackTimer -= Time.deltaTime;

            if (m_AttackTimer <= 0)
            {
                EndAttacking();
            }
        }

        if (m_ExternalForceTimer > 0)
        {
            m_ExternalForceTimer -= Time.deltaTime;
        }




    }

    private void FixedUpdate()
    {
        if (m_BlockNormalAction) return;

        Move();
        Face();
        Animate();
    }

    public void Jump()
    {
        if (m_CharacterController2D.IsGrounded && m_ExternalForceTimer <=0)
        {
            m_JumpForceVector.y = jumpForce;
            m_Rigidbody2D.AddForce(m_JumpForceVector, ForceMode2D.Impulse);
            m_JumpForceVector = Vector2.zero;
        }
    }

    public void SpawnShield()
    {
        Debug.Log("spawn shield");
        shield.Play();
    }

    private void Move()
    {
        if (m_ExternalForceTimer <= 0)
        {
            //set velocity 
            m_Velocity.Set(m_CharacterInput.HorizontalAxis * speed, m_Rigidbody2D.velocity.y);

            //Move rigidbody
            m_Rigidbody2D.velocity = m_Velocity;
        }

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


    public void StartDashing()
    {
        if (!m_CanDash) return;

        if (!m_CharacterController2D.IsGrounded)
        {
            if (m_DashedInAir)
            {
                return;
            }
            else
            {
                m_DashedInAir = true;
            }
        }

        //set timer
        m_DashTimer = dashDuration;

        //enable dash effect
        m_OriginalGravity = m_Rigidbody2D.gravityScale;
        m_Rigidbody2D.gravityScale = 0;
        dashEffect.SetActive(true);
        m_BlockNormalAction = true;
        m_CanDash = false;
        m_Animator.SetBool(m_HashDashPara, true);


        //get direction
        Vector2 direction = m_SpriteRenderer.flipX ? Vector2.left : Vector2.right;

        ////rotate the sprite a little bit
        //if (direction.x > 0)
        //{
        //    m_AlessiaGraphics.rotation = Quaternion.Euler(0, 0, -20);
        //}
        //else
        //{
        //    m_AlessiaGraphics.rotation = Quaternion.Euler(0, 0, 20);
        //}

        //dash
        m_Rigidbody2D.velocity = direction * dashSpeed;


        dashAudioPlayer.PlayRandomSound();

    }


    public void EndDashing()
    {
        //disable dash effect 
        m_Rigidbody2D.gravityScale = m_OriginalGravity;
        dashEffect.SetActive(false);
        m_BlockNormalAction = false;
        m_DashCoolDownTimer = dashCooldDownTime;
        m_Animator.SetBool(m_HashDashPara, false);
        m_AlessiaGraphics.rotation = Quaternion.Euler(0, 0, 0);
    }

    public void StartAttacking()
    {
        if (!m_CanSlash) return;

        //still attacking
        if (m_AttackTimer > 0) return;

        m_AttackTimer = 0.2f;

        //enable attack damage
        m_Slash.EnableDamage();

        slashAudioPlayer.PlayRandomSound();

        //Play attack effect
        if (m_SpriteRenderer.flipX)
        {
            leftSlashEffect.Play();
        }
        else
        {
            rightSlashEffect.Play();
        }
    
    }

    public void EndAttacking()
    {
        slashAudioPlayer.Stop();
        m_Slash.DisableDamage();
    }

    public void GotHit(Damager damager, Damageable damageable)
    {
        //throw player away a little bit
        m_ThrowVector = new Vector2(0, throwSpeed.y);
        Vector2 damagerToThis = damager.transform.position - transform.position;
        m_ThrowVector.x = Mathf.Sign(damagerToThis.x) * -throwSpeed.x;
        m_Rigidbody2D.velocity = Vector2.zero;
        m_Rigidbody2D.AddForce(m_ThrowVector, ForceMode2D.Impulse);
        m_ExternalForceTimer = 0.5f;

        //Set animation
        m_Animator.SetTrigger(m_HashHurtPara);

        //Flicker
        m_Flicker.StartFlickering(damageable.invulnerabilityDuration, timeBetweenFlickering);

        //Shake camera a little
        CameraShaker.Shake(0.15f, 0.3f);

    }

    public void AttackHit(Damager damager, Damageable damageable)
    {
        //push back player a little bit
        Vector2 m_PushBackVector;

        if (!m_SpriteRenderer.flipX)
        {
            //set position of slash contact effect to be displayed
            slashContactTransform.position = transform.position + m_OffsetFromSlashEffectToAlessia;

            m_PushBackVector = new Vector2(-0.8f, 0);
        }
        else
        {
            //set position of slash contact effect to be displayed
            Vector3 m_ReverseOffset = m_OffsetFromSlashEffectToAlessia;
            m_ReverseOffset.x *= -1;
            slashContactTransform.position = transform.position + m_ReverseOffset;

            m_PushBackVector = new Vector2(0.8f, 0);
        }

        //Display slash contact effect
        slashContactTransform.rotation = Quaternion.Euler(0, 0, Random.Range(-50f, 50f));      
        m_SlashContactEffect.Play();

        //Push back
        m_Rigidbody2D.AddForce(m_PushBackVector, ForceMode2D.Impulse);
        m_ExternalForceTimer = 0.1f;

        ////Slowdown time a little bit
        //TimeManager.SlowdownTime(0.2f, 0.2f);


        ////Push damageable object back just a tiny bit
        //Rigidbody2D damageableBody = damageable.GetComponent<Rigidbody2D>();

        //if (damageableBody == null) return;

        //Vector2 damagerToDamageable = damager.transform.position - damageableBody.transform.position;
        //if (damagerToDamageable.x > 0)
        //{
        //    damageableBody.MovePosition(damageableBody.position + new Vector2(-0.2f, 0));
        //}
        //else
        //{
        //    damageableBody.MovePosition(damageableBody.position + new Vector2(0.2f, 0));
        //}


    }

    public void Respawn(bool resetHealth)
    {
        if (m_LastCheckpoint != null)
        {
            m_Rigidbody2D.velocity = Vector2.zero;

            if (resetHealth)
                m_Damageable.SetHealth(m_Damageable.startingHealth);

            //we reset the hurt trigger, as we don't want the player to go back to hurt animation once respawned
            m_Animator.ResetTrigger(m_HashHurtPara);

            m_Flicker.StopFlickering();

            m_SpriteRenderer.flipX = m_LastCheckpoint.respawnFacingLeft;


            transform.position = m_LastCheckpoint.transform.position;

        }

    }

    public void SetChekpoint(Checkpoint checkpoint)
    {
        m_LastCheckpoint = checkpoint;
    }


    public void CanDash(bool canDash)
    {
        m_CanDash = canDash;
    }

    public void CanSlash(bool canSlash)
    {
        m_CanSlash = canSlash;
    }



    public void PlayFootStepAudioPlayer()
    {
        footStepAudioPlayer.PlayRandomSound();
    }

    public void PlayLandAudioPlayer()
    {
        landAudioPlayer.PlayRandomSound();
    }

    public void PlaySlashAudioPlayer()
    {
        slashAudioPlayer.PlayRandomSound();
    }

    public void PlayDashAudioPlayer()
    {
        dashAudioPlayer.PlayRandomSound();
    }

    public void PlayHurtAudioPlayer()
    {
        hurtAudioPlayer.PlayRandomSound();
    }



}
