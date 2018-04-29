using System.Collections.Generic;
using UnityEngine;
using Gamekit2D;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class SimpleCharacterController2D : MonoBehaviour
{
    [Tooltip("The Layers which represent gameobjects that the Character Controller can be grounded on.")]
    public LayerMask groundedLayerMask;
    [Tooltip("The distance down to check for ground.")]
    public float groundedRaycastDistance = 0.1f;

    Rigidbody2D m_Rigidbody2D;
    CapsuleCollider2D m_CapsuleCollider2D;
    ContactFilter2D m_ContactFilter;
    RaycastHit2D[] m_HitBuffer = new RaycastHit2D[5];
    Collider2D[] m_GroundCollider = new Collider2D[3];
    Vector2[] m_RaycastStartPositions = new Vector2[3];
    Vector2 m_NextMovement;

    public bool IsGrounded { get; protected set; }
    public bool IsCeilinged { get; protected set; }
    public Vector2 Velocity { get; protected set; }
    public Rigidbody2D Rigidbody2D { get { return m_Rigidbody2D; } }
    public ContactFilter2D ContactFilter { get { return m_ContactFilter; } }
    public Collider2D[] GroundColliders { get { return m_GroundCollider; } }

    void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        m_CapsuleCollider2D = GetComponent<CapsuleCollider2D>();

        m_ContactFilter.layerMask = groundedLayerMask;
        m_ContactFilter.useLayerMask = true;
        m_ContactFilter.useTriggers = false;

        Physics2D.queriesStartInColliders = false;
    }

    void FixedUpdate()
    {

        CheckCapsuleEndCollisions();
    }

    /// <summary>
    /// This moves a rigidbody and so should only be called from FixedUpdate or other Physics messages.
    /// </summary>
    /// <param name="movement">The amount moved in global coordinates relative to the rigidbody2D's position.</param>
    public void Move(Vector2 movement)
    {
        //use + sign other than = sign because we may want to call this function multiple times in the same frame 
        m_NextMovement += movement;
    }

    /// <summary>
    /// This moves the character without any implied velocity.
    /// </summary>
    /// <param name="position">The new position of the character in global space.</param>
    public void Teleport(Vector2 position)
    {
        m_Rigidbody2D.MovePosition(position);
    }

    /// <summary>
    /// This updates the state of IsGrounded.  It is called automatically in FixedUpdate but can be called more frequently if higher accurracy is required.
    /// </summary>
    /// <summary>
    /// This updates the state of IsGrounded.  It is called automatically in Update but can be called more frequently if higher accurracy is required.
    /// </summary>
    public void CheckCapsuleEndCollisions()
    {
        Vector2 raycastDirection;
        Vector2 raycastStart;
        float raycastDistance;

        raycastStart = m_Rigidbody2D.position + m_CapsuleCollider2D.offset;

        //we add size.x/2 to this line of code because in a bit later, we subtract the  raycastStartBottomCentre to size.x/2
        raycastDistance = m_CapsuleCollider2D.size.x * 0.5f + groundedRaycastDistance * 2f;

        raycastDirection = Vector2.down;

        Vector2 raycastStartBottomCentre = raycastStart + Vector2.down * (m_CapsuleCollider2D.size.y * 0.5f - m_CapsuleCollider2D.size.x * 0.5f);
        m_RaycastStartPositions[0] = raycastStartBottomCentre + Vector2.left * m_CapsuleCollider2D.size.x * 0.5f;
        m_RaycastStartPositions[1] = raycastStartBottomCentre;
        m_RaycastStartPositions[2] = raycastStartBottomCentre + Vector2.right * m_CapsuleCollider2D.size.x * 0.5f;

        int totalCount = 0;

        for (int i = 0; i < m_RaycastStartPositions.Length; i++)
        {
            //Shoot ray
            int count = Physics2D.Raycast(m_RaycastStartPositions[i], raycastDirection, m_ContactFilter, m_HitBuffer, raycastDistance);
            //Debug.DrawRay(m_RaycastStartPositions[i], raycastDirection);

            //get ground collider
            m_GroundCollider[i] = count > 0 ? m_HitBuffer[0].collider : null;

            totalCount += count;
        }

        if (totalCount == 0)
        {
            IsGrounded = false;
        }
        else
        {
            IsGrounded = true;
        }
        
        //reset buffer
        for (int i = 0; i < m_HitBuffer.Length; i++)
        {
            m_HitBuffer[i] = new RaycastHit2D();
        }

    }
}
