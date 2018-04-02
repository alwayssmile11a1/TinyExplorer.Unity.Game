using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamekit2D;

[RequireComponent(typeof(Damager))]
public class LightSphereController : MonoBehaviour {

    public float deactiveDuration = 1f;

    private float m_Timer = 0f;
    private SpriteRenderer m_SpriteRenderer;
    private Damager m_Damager;
    private void Awake()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_Damager = GetComponent<Damager>();
    }

    private void Update()
    {
        if(m_Timer > 0)
        {
            m_Timer -= Time.deltaTime;

            if(m_Timer<0)
            {
                Reactive();
            }
        }
    }

    public void Hit(Damager damager, Damageable damageable)
    {

        Deactive();
        m_Timer = deactiveDuration;
    }


    private void Deactive()
    {
        m_SpriteRenderer.enabled = false;
    }

    private void Reactive()
    {
        m_SpriteRenderer.enabled = true;
        m_Damager.EnableDamage();
    }

}
