using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothStopVFX : MonoBehaviour {

    [Tooltip("Set to a value smaller than 0 if you don't want the gameObject attached to this script to be disabled")]
    public float timeBeforeDisabledAfterStopping = 2f;

    private ParticleSystem[] particleSystems;
    private float m_TimeBeforeDisabled;

    // Use this for initialization
    void Awake () {
        particleSystems = GetComponentsInChildren<ParticleSystem>();
    }

    private void Update()
    {
        if(m_TimeBeforeDisabled>0)
        {
            m_TimeBeforeDisabled -= Time.deltaTime;

            if(m_TimeBeforeDisabled<=0)
            {
                gameObject.SetActive(false);
            }
        }
    }

    public void Stop()
    {
        for (var i = 0; i < particleSystems.Length; i++)
            particleSystems[i].Stop();


        m_TimeBeforeDisabled = timeBeforeDisabledAfterStopping;


    }

}
