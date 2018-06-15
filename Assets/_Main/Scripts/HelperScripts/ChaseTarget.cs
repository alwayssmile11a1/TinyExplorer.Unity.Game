using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Rigidbody2D))]
public class ChaseTarget : MonoBehaviour {

    public Transform target;
    [Tooltip("if set, tag will be used rather than targer transform (the value of target will also be changed)")]
    public bool findTargetByTag = false;
    public string targetTag = "target";
    public Vector3 offsetFromTarget;
    [Tooltip("set to true if you want the offset to be relative with the target sprite facing")]
    public bool offsetBasedOnTargetSpriteFacing = true;
    public bool chaseOnAwake = false;
    public float chaseSpeed = 5f;

    [Header("---------------------------------------------")]
    [Tooltip("If set, the gameObject this component is attached to will be rotated so the right vector points at target's current position")]
    public bool orientToTarget = true;
    [Tooltip("0 means the gameObject that this component is attached to aligns horizontally")]
    public float rotationZComparedToHorizontal = 0f;
    public float rotationSpeed = 200f;



    private SpriteRenderer m_TargetSpriteRenderer;
    private Vector3 m_Offset;
    private Rigidbody2D m_Rigidbody2D;
    private bool m_CanChase;

    // Use this for initialization
    void Awake()
    {
        if(findTargetByTag)
        {
            target = GameObject.FindGameObjectWithTag(targetTag).transform;
        }

        if (target != null)
        {
            m_TargetSpriteRenderer = target.GetComponent<SpriteRenderer>();
        }
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        if (chaseOnAwake) m_CanChase = true;
    }

    void FixedUpdate()
    {
        if (target == null) return;

        Orient();
        Chase();
        
    }

    private void Orient()
    {
        if (!orientToTarget) return;

        //Get offset from target
        m_Offset = offsetFromTarget;
        if (offsetBasedOnTargetSpriteFacing && m_TargetSpriteRenderer != null)
        {
            if (m_TargetSpriteRenderer.flipX)
            {
                m_Offset.x = offsetFromTarget.x * -1;
            }
            else
            {
                m_Offset.x = offsetFromTarget.x;
            }
        }

        //direction from target to the gameObject
        Vector2 direction = (target.position + m_Offset - transform.position).normalized;

        ////rotate
        //float rotationZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        //Quaternion desiredRotation = Quaternion.Euler(0, 0, rotationZ - rotationZComparedToHorizontal);
        //transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime * rotationSpeed);

        //OR use cross product
        float rotateAmount = Vector3.Cross(direction, Quaternion.Euler(0,0,rotationZComparedToHorizontal) * transform.right).z;
        m_Rigidbody2D.angularVelocity = -rotateAmount * rotationSpeed;

    }

    private void Chase()
    {
        if (!m_CanChase) return;

        m_Rigidbody2D.velocity = Quaternion.Euler(0, 0, rotationZComparedToHorizontal) * transform.right * chaseSpeed;
    }

    public void StartChasing()
    {
        m_CanChase = true;
    }

    public void StopChasing()
    {
        m_CanChase = false;
    }


#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {

        if (target == null) return;

        Handles.color = new Color(1.0f, 0, 0, 0.5f);
        Handles.DrawSolidDisc(target.position + offsetFromTarget, Vector3.back, 0.1f);

    }
#endif

}
