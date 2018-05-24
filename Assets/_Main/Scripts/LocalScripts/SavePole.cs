using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamekit2D;

[RequireComponent(typeof(Collider2D))]
public class SavePole : MonoBehaviour
{
    public string saveSuccessfullyEffect = "SavedEffect";

    private Animator m_Animator;

    private int m_HashSavedEffect;

    private int m_HashActivePara = Animator.StringToHash("Active");
    private int m_HashSaved = Animator.StringToHash("Saved");

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
        m_HashSavedEffect = VFXController.StringToHash(saveSuccessfullyEffect);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        AlessiaController alessia = collision.GetComponent<AlessiaController>();

        if (alessia != null)
        {
            m_Animator.SetBool(m_HashActivePara, true);
            alessia.SetSavePole(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        AlessiaController alessia = collision.GetComponent<AlessiaController>();

        if (alessia != null)
        {
            m_Animator.SetBool(m_HashActivePara, false);
            alessia.SetSavePole(null);
        }


    }
    
    public void TriggerSavedEffect()
    {
        m_Animator.SetTrigger(m_HashSaved);
        m_Animator.SetBool(m_HashActivePara, false);
        VFXController.Instance.Trigger(m_HashSavedEffect, transform.position + transform.up * 0.8f, 0, false, null);
    }

}
