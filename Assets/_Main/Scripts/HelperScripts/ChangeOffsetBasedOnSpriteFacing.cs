using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeOffsetBasedOnSpriteFacing : MonoBehaviour {

    public Transform target;
    public SpriteRenderer spriteRenderer;

    private Vector3 m_Offset;
    private Vector3 m_ReverseOffset;

	// Use this for initialization
	void Start () {

        m_Offset = transform.position - target.position;
        m_ReverseOffset = new Vector3(-m_Offset.x, m_Offset.y, m_Offset.z);
	}
	
	// Update is called once per frame
	void Update () {
		
        if(!spriteRenderer.flipX)
        {
            transform.position = target.position + m_Offset;
        }
        else
        {
            transform.position = target.position + m_ReverseOffset;
        }

	}



}
