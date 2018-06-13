using Gamekit2D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniCollectableHealth : MonoBehaviour {

    public float fillAmount = 0.05f;

    public string CollectedEffectName = "Yellow_Explosion";


    [HideInInspector]
    public static int EffectHash;

    private HealthUI m_HealthUI;
    private Rigidbody2D m_RigidBody2D;


    private void Awake()
    {
        m_HealthUI = FindObjectOfType<HealthUI>();
        EffectHash = VFXController.StringToHash(CollectedEffectName);
        m_RigidBody2D = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (m_HealthUI == null) return;

        AlessiaController alessia = collision.GetComponent<AlessiaController>();

        if(alessia !=null )
        {
            m_HealthUI.FillHitPointUI(fillAmount);
            gameObject.SetActive(false);
            VFXController.Instance.Trigger(EffectHash, transform.position, 0, false, null, null);
        }


    }


    ////private void OnEnable()
    ////{
    ////    m_RigidBody2D.AddForce(new Vector2(Random.Range(-3.0f,3.0f), Random.Range(2.0f, 3.0f)), ForceMode2D.Impulse);
    ////}

}
