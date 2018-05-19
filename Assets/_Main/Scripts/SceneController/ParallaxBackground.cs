using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour {

    public float smoothSpeed = 1f;

    private Vector3 m_PreviousCameraPosition;


	// Use this for initialization
	void Awake () {
        m_PreviousCameraPosition = Camera.main.transform.position;
	}
	
	// Update is called once per frame
	void Update () {

        float newPositionX = transform.position.x + (Camera.main.transform.position.x - m_PreviousCameraPosition.x) * 20 / (transform.position.z - Camera.main.transform.position.z);


        transform.position = Vector3.Lerp(transform.position, new Vector3(newPositionX, transform.position.y, transform.position.z), Time.deltaTime * smoothSpeed);

        m_PreviousCameraPosition = Camera.main.transform.position;
	}
}
