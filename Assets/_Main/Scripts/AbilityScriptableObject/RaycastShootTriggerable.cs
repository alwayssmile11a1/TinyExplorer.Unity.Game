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
    

    private RaycastHit hit;
    private Ray shootRay = new Ray();
    private int shootableMask;
    private LineRenderer laserRenderer;
    private WaitForSeconds laserDisplayTime = new WaitForSeconds(0.02f);

    private float m_Timer;
    private bool canShoot = true;

    private void Awake()
    {
        laserRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if (!canShoot)
        {
            if (m_Timer > 0)
            {
                m_Timer -= Time.deltaTime;
            }
            else
            {
                canShoot = true;
            }
        }
    }


    // Use this for initialization
    public void Initialize () {
        shootableMask = shootableLayers.value;
        laserRenderer.colorGradient = laserColor;
        //laserRenderer.useWorldSpace = true;
	}


    public void Trigger()
    {

        if (!canShoot) return;

        InternalTrigger();

        //reset timer
        m_Timer = 1 / fireRate;
        canShoot = false;

    }


    private void InternalTrigger()
    {
        //setup laser line
        laserRenderer.SetPosition(0, startingShootPosition.position);

        //ray
        shootRay.origin = startingShootPosition.position;
        shootRay.direction = startingShootPosition.right;

        //start displaying shoot effect
        StartCoroutine(DisplayShotEffect());


        //if the laser hit something
        if (Physics.Raycast(shootRay, out hit, range, shootableMask))
        {

        }
        else
        {
            laserRenderer.SetPosition(1, shootRay.origin + shootRay.direction * range);

        }
    }

    


    private IEnumerator DisplayShotEffect()
    {
        //setup laser line
        laserRenderer.enabled = true;

        yield return laserDisplayTime;

        laserRenderer.enabled = false;

    }

}
