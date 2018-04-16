using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPointController : MonoBehaviour {


    public float maxMoveDownOffset = 1f;
    public float maxMoveUpOffset = 1f;
    public float moveUpDownSpeed = 5f;
    public float moveToOriginSpeed = 5f;
    public float delayBeforeMoving = 1f;


    private Vector3 m_OriginalLocalPosition;
    private float m_HoldDownTimer;
    private float m_HoldUpTimer;

    private void Start()
    {
        m_OriginalLocalPosition = transform.localPosition;
    }

    private void Update()
    {
        if (transform.localPosition != m_OriginalLocalPosition)
        {
            transform.localPosition = Vector3.Slerp(transform.localPosition, m_OriginalLocalPosition, moveToOriginSpeed * Time.deltaTime);
        }
        if(Input.GetKey(KeyCode.DownArrow))
        {
            m_HoldDownTimer += Time.deltaTime;
        }
        else
        {
            m_HoldDownTimer = 0;
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            m_HoldUpTimer += Time.deltaTime;
        }
        else
        {
            m_HoldUpTimer = 0;
        }

        if (m_HoldDownTimer >= delayBeforeMoving)
        {
            MoveDown();
        }

        if(m_HoldUpTimer >= delayBeforeMoving)
        {
            MoveUp();
        }

    }



    public void MoveDown()
    {
        //if (m_HoldDownTimer >= delayBeforeMoving)
        {
            if (m_OriginalLocalPosition.y - transform.localPosition.y > maxMoveDownOffset)
            {
                return;
            }

            Vector3 newPosition = transform.localPosition;
            newPosition.y -= 5f * Time.deltaTime;

            transform.localPosition = newPosition;
        }
    }
    
    public void MoveUp()
    {
        //if (m_HoldUpTimer >= delayBeforeMoving)
        {
            if (transform.localPosition.y - m_OriginalLocalPosition.y > maxMoveUpOffset)
            {
                return;
            }

            Vector3 newPosition = transform.localPosition;
            newPosition.y += 5f * Time.deltaTime;

            transform.localPosition = newPosition;
        }
    }


}
