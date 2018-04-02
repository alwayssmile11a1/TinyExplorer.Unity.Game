using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSpheresController : MonoBehaviour {


    public GameObject[] lightSpheres;
	


	void Start () {

        //layout lightspheres around player
        float angleOffset = 360.0f / lightSpheres.Length;
        for (int i = 0; i < lightSpheres.Length; i++)
        {
            RotateAroundTarget rotateAroundTargetComponent = lightSpheres[i].GetComponent<RotateAroundTarget>();

            rotateAroundTargetComponent.SetStartPosition(i * angleOffset, rotateAroundTargetComponent.distanceFromRotatingPoint);

        }


	}
	
	// Update is called once per frame
	void Update () {
		
	}





}
