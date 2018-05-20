using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamekit2D;

[RequireComponent(typeof(CharacterInput))]
[RequireComponent(typeof(CharacterController2D))]
public class AlessiaController : MonoBehaviour, IDataSaveable {

    public float speed = 5f;
    public float jumpSpeed = 8.5f;
    public float jumpAbortSpeedReduction = 20f;
    public float gravity = 15f;

    [Tooltip("Throw speed when get hit")]
    public Vector2 throwSpeed = new Vector2(3, 3);

    [Tooltip("push back speed when hit something")]
    public float pushBackSpeed = 1f;

    [Header("Dash")]
    public GameObject dashEffect;
    public float dashSpeed = 5f;
    public float dashDuration = 1f;
    public float dashCooldDownTime = 1f;

    [Header("Slash")]
    public Damager leftDamager;
    public Damager rightDamager;
    public ParticleSystem leftSlashEffect;
    public ParticleSystem rightSlashEffect;
    public Transform slashContactTransform;
    public string slashHitEffectName="ExplodingHitEffect";

    [Header("Audio")]
    public RandomAudioPlayer footStepAudioPlayer;
    public RandomAudioPlayer slashAudioPlayer;
    public RandomAudioPlayer landAudioPlayer;
    public RandomAudioPlayer dashAudioPlayer;
    public RandomAudioPlayer hurtAudioPlayer;
    public RandomAudioPlayer slashHitAudioPlayer;


    [Header("Misc")]
    public float timeBetweenFlickering = 0;

    private CharacterController2D m_CharacterController2D;
    private Vector2 m_Velocity = new Vector2();
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
    private int m_HashOnLadderPara = Animator.StringToHash("isOnLadder");


    private Vector2 m_MoveVector;
    private bool m_IsJumpHolding;
    private int m_HashSlashHitEffect;
    private bool m_BlockNormalAction;
    private float m_DashCoolDownTimer;
    private float m_DashTimer;
    private bool m_CanDash = true;
    private bool m_CanSlash = true;
    private bool m_CanClimb = false;
    //Allow dash in air only one time
    private bool m_DashedInAir = false;

    private float m_AttackTimer;
    private float m_ExternalForceTimer;


    private Checkpoint m_LastCheckpoint = null;
    private Damageable m_Damageable;


    private const float k_GroundedStickingVelocityMultiplier = 3f;    // This is to help the character stick to vertically moving platforms.

    private void Awake () {

        if(SavedDataManager.LoadAllData())
        {

        }


        //m_Rigidbody2D = GetComponent<Rigidbody2D>();
        m_SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        m_AlessiaGraphics = m_SpriteRenderer.gameObject.transform;
        m_Animator = GetComponent<Animator>();
        m_CharacterController2D = GetComponent<CharacterController2D>();
        m_CharacterInput = GetComponent<CharacterInput>();
        m_Flicker = m_SpriteRenderer.gameObject.AddComponent<Flicker>();
        m_SlashContactEffect = slashContactTransform.GetComponentInChildren<ParticleSystem>();
        m_OffsetFromSlashEffectToAlessia = slashContactTransform.position - transform.position;
        m_Damageable = GetComponent<Damageable>();


        m_HashSlashHitEffect = VFXController.StringToHash(slashHitEffectName);

    }

    private void Update()
    {

        if(m_CharacterController2D.IsGrounded)
        {
            m_DashedInAir = false;
        }

        if(m_DashCoolDownTimer>0)
        {
            m_DashCoolDownTimer -= Time.deltaTime;

            if (m_DashCoolDownTimer <= 0)
            {
                //if we haven't grounded yet, can not dash more (allow dash only 1 time in air)
                if (m_DashedInAir)
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

        Move();
        Face();
        Animate();
    }

    public void Jump()
    {
        if (m_CharacterController2D.IsGrounded && !m_BlockNormalAction && m_ExternalForceTimer <=0)
        {
            SetVerticalMovement(jumpSpeed);
        }
    }

    public void JumpHeld()
    {
        m_IsJumpHolding = true;
    }

    public void JumpRelease()
    {
        m_IsJumpHolding = false;
    }

    //public void SpawnShield()
    //{
    //    Debug.Log("spawn shield");
    //    shield.Play();
    //}

    private void Move()
    {
        if (!m_BlockNormalAction)
        {
            UpdateJump();

            if (!m_CharacterController2D.IsGrounded)
            {
                AirborneVerticalMovement();
            }
            else
            {
                GroundedVerticalMovement();
            }

            if (m_ExternalForceTimer <= 0)
            {
                SetHorizontalMovement(m_CharacterInput.HorizontalAxis * speed);
            }
        }
        
        m_CharacterController2D.Move(m_MoveVector * Time.fixedDeltaTime);

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

    
    public void SetMoveVector(Vector2 newMoveVector)
    {
        m_MoveVector = newMoveVector;
    }

    public void SetHorizontalMovement(float newHorizontalMovement)
    {
        m_MoveVector.x = newHorizontalMovement;
    }

    public void SetVerticalMovement(float newVerticalMovement)
    {
        m_MoveVector.y = newVerticalMovement;
    }

    public void IncrementMovement(Vector2 additionalMovement)
    {
        m_MoveVector += additionalMovement;
    }

    public void IncrementHorizontalMovement(float additionalHorizontalMovement)
    {
        m_MoveVector.x += additionalHorizontalMovement;
    }

    public void IncrementVerticalMovement(float additionalVerticalMovement)
    {
        m_MoveVector.y += additionalVerticalMovement;
    }

    public void UpdateJump()
    {
        if (!m_IsJumpHolding && m_MoveVector.y > 0.0f)
        {
            m_MoveVector.y -= jumpAbortSpeedReduction * Time.deltaTime;
        }
    }

    public void GroundedVerticalMovement()
    {
        m_MoveVector.y -= gravity * Time.deltaTime;

        if (m_MoveVector.y < -gravity * Time.deltaTime * k_GroundedStickingVelocityMultiplier)
        {
            m_MoveVector.y = -gravity * Time.deltaTime * k_GroundedStickingVelocityMultiplier;
        }
    }

    public void AirborneVerticalMovement()
    {
        if (Mathf.Approximately(m_MoveVector.y, 0f) || m_CharacterController2D.IsCeilinged && m_MoveVector.y > 0f)
        {
            m_MoveVector.y = 0f;
        }
        m_MoveVector.y -= gravity * Time.deltaTime;
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
        //m_Rigidbody2D.velocity = direction * dashSpeed;
        SetMoveVector(direction * dashSpeed);
        

        dashAudioPlayer.PlayRandomSound();

    }


    public void EndDashing()
    {
        //disable dash effect 
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


        slashAudioPlayer.PlayRandomSound();

        //Play attack effect
        if (m_SpriteRenderer.flipX)
        {
            leftDamager.EnableDamage();         
            leftSlashEffect.Play();
        }
        else
        {
            rightDamager.EnableDamage();
            rightSlashEffect.Play();
        }
    
    }

    public void EndAttacking()
    {
        slashAudioPlayer.Stop();
        leftDamager.DisableDamage();
        rightDamager.DisableDamage();
    }

    public void GotHit(Damager damager, Damageable damageable)
    {
        //throw player away a little bit
        m_ThrowVector = new Vector2(0, throwSpeed.y);
        Vector2 damagerToThis = damager.transform.position - transform.position;
        m_ThrowVector.x = Mathf.Sign(damagerToThis.x) * -throwSpeed.x;
        SetMoveVector(m_ThrowVector);
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
        OnAttackHit(damager);

        //VFXController.Instance.Trigger(m_HashSlashHitEffect, damageable.transform.position, 0, false, null);

        if (slashHitAudioPlayer)
        {
            slashHitAudioPlayer.PlayRandomSound();
        }

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

    public void AttackHit(Damager damager)
    {
        OnAttackHit(damager);

        //for (int i = 0; i < m_SlashHitResults.Length; i++)
        //{
        //    m_SlashHitResults[i] = new RaycastHit2D();
        //}

        //Physics2D.Raycast(transform.position, m_SpriteRenderer.flipX ? Vector2.left : Vector2.right, damager.GetContactFilter(), m_SlashHitResults);
        //VFXController.Instance.Trigger(m_HashSlashHitEffect, slashContactTransform.position, 0, false, null);

        if(slashHitAudioPlayer)
        {
            UnityEngine.Tilemaps.TileBase surfaceHit = PhysicsHelper.FindTileForOverride(damager.LastHit, slashContactTransform.position, m_SpriteRenderer.flipX ? Vector2.left : Vector2.right);


            slashHitAudioPlayer.PlayRandomSound(surfaceHit);

        }

    }

    private void OnAttackHit(Damager damager)
    {
        //push back player a little bit
        float pushSpeed;

        if (rightDamager.CanDamage() == true)
        {
            //set position of slash contact effect to be displayed
            slashContactTransform.position = transform.position + m_OffsetFromSlashEffectToAlessia;

            pushSpeed = -pushBackSpeed;
        }
        else
        {
            //set position of slash contact effect to be displayed
            Vector3 m_ReverseOffset = m_OffsetFromSlashEffectToAlessia;
            m_ReverseOffset.x *= -1;
            slashContactTransform.position = transform.position + m_ReverseOffset;

            pushSpeed = pushBackSpeed;
        }

        //Display slash contact effect
        slashContactTransform.rotation = Quaternion.Euler(0, 0, Random.Range(-50f, 50f));
        m_SlashContactEffect.Play();

        //Push back
        //m_Rigidbody2D.AddForce(m_PushBackVector, ForceMode2D.Impulse);

        SetHorizontalMovement(pushSpeed);

        m_ExternalForceTimer = 0.1f;

        CameraShaker.Shake(0.05f, 0.05f);


        
    }

    public void OnPickUpNewAbility()
    {
        //VFXController.Instance.Trigger(VFXController.StringToHash("Implode"), transform.position + Vector3.up *0.6f, 0.1f, false, null);
        VFXController.Instance.Trigger(VFXController.StringToHash("Yellow_Explosion"), transform.position + Vector3.up * 0.6f, 0, false, null);
    }

    public void OnDie(Damager damager, Damageable damageable)
    {
        StartCoroutine(DieRespawnCoroutine(true));
    }

    private IEnumerator DieRespawnCoroutine(bool resetHealth)
    {
        if (m_LastCheckpoint != null)
        {
            if (m_LastCheckpoint.forceResetGame)
            {
                if (resetHealth)
                    m_Damageable.SetHealth(m_Damageable.startingHealth);
                SceneController.RestartZoneAtPosition(new Vector3(m_LastCheckpoint.transform.position.x, m_LastCheckpoint.transform.position.y, transform.position.z));
            }
            else
            {
                yield return new WaitForSeconds(0.2f); //wait one second before respawing
                yield return StartCoroutine(ScreenFader.FadeSceneOut());
                Respawn(resetHealth);
                yield return new WaitForSeconds(0.3f);
                yield return StartCoroutine(ScreenFader.FadeSceneIn());
            }
        }

    }

    public void Respawn(bool resetHealth)
    {

        SetMoveVector(Vector2.zero);

        if (resetHealth)
            m_Damageable.SetHealth(m_Damageable.startingHealth);

        //we reset the hurt trigger, as we don't want the player to go back to hurt animation once respawned
        m_Animator.ResetTrigger(m_HashHurtPara);

        m_Flicker.StopFlickering();

        m_SpriteRenderer.flipX = m_LastCheckpoint.respawnFacingLeft;

        transform.position = m_LastCheckpoint.transform.position;

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("ladder"))
        {
            m_CanClimb = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag.Equals("ladder"))
        {
            m_CanClimb = false;
        }
    }

    public void SaveData()
    {
            
    }

    public void LoadData()
    {
        
    }
}
