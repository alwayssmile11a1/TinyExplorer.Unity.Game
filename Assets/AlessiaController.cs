using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class AlessiaController : MonoBehaviour {

    [Tooltip("The Layers which represent gameobjects that the Character Controller can be grounded on.")]
    public LayerMask groundedLayerMask;
    [Tooltip("The distance down to check for ground.")]
    public float groundedRaycastDistance = 0.1f;
    public float speed = 5f;
    public float jumpForce = 400f;

    public FireFlyController fireFlyController;

    


    public KeyCode jumpButton;
    public KeyCode shootButton;

    private Vector2 m_Velocity = new Vector2();
    private Rigidbody2D m_Rigidbody2D;
    private float m_HorizontalMovement;
    private float m_VerticalForce;
    private Animator m_Animator;
    private CapsuleCollider2D m_CapsuleCollider2D;
    private bool isGrounded;
    private ContactFilter2D m_ContactFilter;
    private Vector2[] m_RaycastStartPositions = new Vector2[3];
    private RaycastHit2D[] m_HitBuffer = new RaycastHit2D[5];
    private SpriteRenderer m_SpriteRenderer;

    // Use this for initialization
    void Awake () {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        m_CapsuleCollider2D = GetComponent<CapsuleCollider2D>();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_Animator = GetComponent<Animator>();
        m_ContactFilter.layerMask = groundedLayerMask;
        m_ContactFilter.useLayerMask = true;
        m_ContactFilter.useTriggers = false;
        
        Physics2D.queriesStartInColliders = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Get horizontal move
        m_HorizontalMovement = Input.GetAxis("Horizontal");

        //jump only if grounded
        if (Input.GetKeyDown(jumpButton) && isGrounded)
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

        //Check grounded
        CheckCapsuleEndCollisions();

        
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
        m_Animator.SetBool("grounded", isGrounded);
        m_Animator.SetFloat("velocityX", Mathf.Abs(m_HorizontalMovement));
    }

    private void Shoot()
    {
        if(fireFlyController!=null)
        {
           fireFlyController.Trigger();
        }
    }


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

        int count = 0;

        for (int i = 0; i < m_RaycastStartPositions.Length; i++)
        {
            //Shoot ray
            count += Physics2D.Raycast(m_RaycastStartPositions[i], raycastDirection, m_ContactFilter, m_HitBuffer, raycastDistance);
            Debug.DrawRay(m_RaycastStartPositions[i], raycastDirection);

            if (count > 0)
            {
                isGrounded = true;
                return;
            }

        }

        if(count==0)
        {
            isGrounded = false;
        }

    }


}
