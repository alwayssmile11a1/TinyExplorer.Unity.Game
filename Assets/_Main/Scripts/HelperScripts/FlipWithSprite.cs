using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipWithSprite : MonoBehaviour {

    public SpriteRenderer spriteRenderer;

    [Tooltip("if set, the position of this gameobject will be changed appropriately with the sprite facing")]
    public bool positionBasedOnSpriteFacing = true;


    private Vector3 m_OffsetFromParent;
    private Vector3 m_ReverseOffsetFromParent;
    private Transform m_Target;

    private void Awake()
    {
        m_Target = spriteRenderer.gameObject.transform;
        m_OffsetFromParent = transform.position - m_Target.position;
        m_ReverseOffsetFromParent = m_OffsetFromParent;
        m_ReverseOffsetFromParent.x *= -1;
    }

    // Update is called once per frame
    private void Update () {

        if (!spriteRenderer.flipX && transform.localScale.x == -1)
        {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y,transform.localScale.z);

            if(positionBasedOnSpriteFacing)
            {
                transform.position = m_Target.position + m_OffsetFromParent;
            }

        }

        if (spriteRenderer.flipX && transform.localScale.x == 1)
        {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);

            if(positionBasedOnSpriteFacing)
            {
                transform.position = m_Target.position + m_ReverseOffsetFromParent;
            }

        }

    }

    

}
