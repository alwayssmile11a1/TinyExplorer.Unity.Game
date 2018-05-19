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

        float parallaxMultiplier = 20 / (transform.position.z - Camera.main.transform.position.z);

        float newPositionX = transform.position.x + (Camera.main.transform.position.x - m_PreviousCameraPosition.x) * parallaxMultiplier;
        float newPositionY = transform.position.y + (Camera.main.transform.position.y - m_PreviousCameraPosition.y) * parallaxMultiplier;

        transform.position = Vector3.Lerp(transform.position, new Vector3(newPositionX, newPositionY, transform.position.z), Time.deltaTime * smoothSpeed);

        m_PreviousCameraPosition = Camera.main.transform.position;
	}
}
