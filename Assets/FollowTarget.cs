using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Follow a target
/// </summary>
public class FollowTarget : MonoBehaviour {

    
    public Transform target;
    public float speed = 5f;
    public Vector3 offset;

    [Tooltip("set to true if you want the offset to relative with the target sprite facing")]
    public bool offsetBasedOnTargetSpriteFacing = true;

    private SpriteRenderer m_TargetSpriteRenderer;
    private Vector3 m_Offset;

	// Use this for initialization
	void Start () {

        m_TargetSpriteRenderer = target.GetComponent<SpriteRenderer>();
        
	}
	
	// Update is called once per frame
	void Update () {

        m_Offset = offset;
        if (offsetBasedOnTargetSpriteFacing && m_TargetSpriteRenderer!=null)
        {
            if(m_TargetSpriteRenderer.flipX)
            {
                m_Offset.x = offset.x * -1;
            }
            else
            {
                m_Offset.x = offset.x;
            }
        }


        transform.position = Vector3.Slerp(transform.position, target.position + m_Offset, speed * Time.deltaTime);

	}
}
