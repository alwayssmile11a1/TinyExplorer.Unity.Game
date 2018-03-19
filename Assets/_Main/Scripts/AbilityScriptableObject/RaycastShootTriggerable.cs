using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RaycastShootTriggerable : MonoBehaviour {

    //these variable will be driven by RayCastWeaponSO script
    [HideInInspector] public float range;
    [HideInInspector] public Gradient laserColor;

    public Transform startingShootPosition;

    private RaycastHit hit;
    private Ray shootRay = new Ray();
    private int shootableMask;
    private LineRenderer laserRenderer;
    private WaitForSeconds laserDisplayTime = new WaitForSeconds(0.02f);


	// Use this for initialization
	public void Initialize () {
        shootableMask = LayerMask.GetMask("Shootable");
        laserRenderer = GetComponent<LineRenderer>();
        laserRenderer.colorGradient = laserColor;
        laserRenderer.useWorldSpace = true;
	}


    public void Fire()
    {
        //setup laser line
        laserRenderer.SetPosition(0, startingShootPosition.position);

        //ray
        shootRay.origin = startingShootPosition.position;
        shootRay.direction = startingShootPosition.forward;

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
