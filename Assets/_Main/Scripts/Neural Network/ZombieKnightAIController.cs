using Gamekit2D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieKnightAIController : MonoBehaviour
{

    public float jumpSpeed = 10f;
    public float moveSpeed = 5f;
    public float jumpAbortSpeedReduction = 20f;
    public float gravity = 15f;
    private Animator m_Animator;
    private Rigidbody2D m_Rigidbody2D;
    private CharacterController2D m_CharacterController2D;
    private Vector2 m_MoveVector;

    private const float k_GroundedStickingVelocityMultiplier = 3f;    // This is to help the character stick to vertically moving platforms.

    private int m_HashGroundedPara = Animator.StringToHash("Grounded");
    private int m_HashRunPara = Animator.StringToHash("Run");

    private void Awake()
    {
        m_Animator = GetComponentInChildren<Animator>();
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        m_CharacterController2D = GetComponent<CharacterController2D>();
    }

    private void FixedUpdate()
    {

        Move();
        Animate();
    }

    public void MoveRight(float direction)
    {
        //var nextPosition = m_Rigidbody2D.position + Vector2.right * direction * moveSpeed * Time.fixedDeltaTime;
        //m_Rigidbody2D.MovePosition(nextPosition);
        //m_Animator.SetBool("Run",true);

        SetHorizontalMovement(direction * moveSpeed);
    }

    public void Jump()
    {
        if (m_CharacterController2D.IsGrounded)
        {
            SetVerticalMovement(jumpSpeed);
        }
    }

    private void Move()
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

        m_CharacterController2D.Move(m_MoveVector * Time.fixedDeltaTime);

    }

    private void Animate()
    {
        //m_Animator.SetBool(m_HashGroundedPara, m_CharacterController2D.IsGrounded);
        if(m_CharacterController2D.IsGrounded)
        {
            m_Animator.SetBool(m_HashRunPara, Mathf.Abs(m_MoveVector.x) > 0);
        }
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
        if (m_MoveVector.y > 0.0f)
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

}
