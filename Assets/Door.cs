using Gamekit2D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(TransitionPoint))]
public class Door : MonoBehaviour {


    public bool canTransition = true;

    //public string enterDoorEffect = "Effect";


    //private int m_HashSaved = Animator.StringToHash("Saved");
    //private int m_EnterDoorEffect;
    private Animator m_Animator;
    private int m_HashActivePara = Animator.StringToHash("Active");
    private TransitionPoint m_TransitionPoint;

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
        m_TransitionPoint = GetComponent<TransitionPoint>();
        //m_EnterDoorEffect = VFXController.StringToHash(enterDoorEffect);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        AlessiaController alessia = collision.GetComponent<AlessiaController>();

        if (alessia != null)
        {
            m_Animator.SetBool(m_HashActivePara, true);

            if (canTransition)
            {
                alessia.SetDoor(this);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        AlessiaController alessia = collision.GetComponent<AlessiaController>();

        if (alessia != null)
        {
            m_Animator.SetBool(m_HashActivePara, false);
            if (canTransition)
            {
                alessia.SetDoor(null);
            }
        }


    }

    public void Transition()
    {
        m_TransitionPoint.Transition();
    }

    //public void TriggerEnterDoorEffect()
    //{
    //    m_Animator.SetTrigger(m_HashSaved);
    //    m_Animator.SetBool(m_HashActivePara, false);
    //    VFXController.Instance.Trigger(m_EnterDoorEffect, transform.position + transform.up * 0.8f, 0, false, null);
    //}
}
