using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeXOffsetBasedOnSpriteFacing : MonoBehaviour {

    public Transform target;
    public SpriteRenderer spriteRenderer;

    private float m_Offset;
    //private Vector3 m_ReverseOffset;

	// Use this for initialization
	void Start () {

        m_Offset = transform.position.x - target.position.x;
        //m_ReverseOffset = new Vector3(-m_Offset.x, m_Offset.y, m_Offset.z);
	}
	
	// Update is called once per frame
	void Update () {
		
        if(!spriteRenderer.flipX && !Mathf.Approximately(transform.position.x, target.position.x + m_Offset))
        {
            transform.position = new Vector3( target.position.x + m_Offset, transform.position.y, transform.position.z);
        }

        if (spriteRenderer.flipX && !Mathf.Approximately(transform.position.x, target.position.x - m_Offset))
        {
            transform.position = new Vector3(target.position.x - m_Offset, transform.position.y, transform.position.z);
        }

	}



}
