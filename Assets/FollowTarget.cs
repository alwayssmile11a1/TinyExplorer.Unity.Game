using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Follow a target
/// </summary>
public class FollowTarget : MonoBehaviour {

    
    public Transform target;
    public float speed = 5f;

    [Tooltip("use usedOffset variable to adjust what offset will be used")]
    public Vector3[] offsets;
    public int usedOffset = 0;

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

        m_Offset = offsets[usedOffset];

        if (offsetBasedOnTargetSpriteFacing && m_TargetSpriteRenderer!=null)
        {
            if(m_TargetSpriteRenderer.flipX)
            {
                m_Offset.x = offsets[usedOffset].x * -1;
            }
            else
            {
                m_Offset.x = offsets[usedOffset].x;
            }
        }


        transform.position = Vector3.Slerp(transform.position, target.position + m_Offset, speed * Time.deltaTime);

	}

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        for (int i = 0; i < offsets.Length; i++)
        {
            Handles.color = new Color(1.0f, 0, 0, 0.1f);
            Handles.DrawSolidDisc(target.position + offsets[i], Vector3.back, 0.1f);
        }

    }
#endif
}

