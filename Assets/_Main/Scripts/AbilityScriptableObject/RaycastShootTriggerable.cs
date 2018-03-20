using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RaycastShootTriggerable : MonoBehaviour {

    //these variable will be driven by RayCastWeaponSO script
    [HideInInspector] public float range;
    [HideInInspector] public Gradient laserColor;
    [HideInInspector] public float fireRate;

    public Transform startingShootPosition;
    public LayerMask shootableLayers;
    

    private RaycastHit2D m_RaycastHit2D;
    private int m_ShootableMask;
    private LineRenderer m_LaserRenderer;
    private WaitForSeconds m_LaserDisplayTime = new WaitForSeconds(0.02f);
    private bool m_DisplayEffectUntilShootEnd = false;
    private float m_Timer;
    private bool m_CanShoot = true;

    private void Awake()
    {
        m_LaserRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {

        if (m_Timer > 0)
        {
            m_Timer -= Time.deltaTime;
        }

    }


    // Use this for initialization
    public void Initialize () {
        m_ShootableMask = shootableLayers.value;
        m_LaserRenderer.colorGradient = laserColor;
        //laserRenderer.useWorldSpace = true;
	}


    public void Trigger()
    {
        if (!m_CanShoot) return;

        if (fireRate <= 0)
        {
            m_DisplayEffectUntilShootEnd = true;
            m_Timer = Time.deltaTime + 0.02f;
        }
        else
        {
            m_DisplayEffectUntilShootEnd = false;
            if (m_Timer > 0) return;
        }

        InternalTrigger();

        if (!m_DisplayEffectUntilShootEnd)
        {
            ResetTimer();
        }
    }

    private void ResetTimer()
    {
        //reset timer
        m_Timer = 1 / fireRate;
        //canShoot = false;
    }


    private void InternalTrigger()
    {

        //setup laser line
        m_LaserRenderer.SetPosition(0, startingShootPosition.position);



        //start displaying shoot effect
        StartCoroutine(DisplayShotEffect());


        //if the laser hit something
        m_RaycastHit2D = Physics2D.Raycast(startingShootPosition.position, startingShootPosition.right, range, m_ShootableMask);

        if (m_RaycastHit2D)
        {
            
            m_LaserRenderer.SetPosition(1, m_RaycastHit2D.point);
        }
        else
        {
            m_LaserRenderer.SetPosition(1, startingShootPosition.position + startingShootPosition.right * range);

        }
    }

    


    private IEnumerator DisplayShotEffect()
    {
        //setup laser line
        m_LaserRenderer.enabled = true;

        if (!m_DisplayEffectUntilShootEnd)
        {
            yield return m_LaserDisplayTime;
        }
        else
        {
            yield return new WaitUntil(() => m_Timer < 0);
        }

        m_LaserRenderer.enabled = false;

    }

    public void SetCanShoot(bool canShoot)
    {
        this.m_CanShoot = canShoot;
    }

}
