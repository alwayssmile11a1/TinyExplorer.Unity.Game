using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateWithMath : MonoBehaviour {
    public float speed = 2;
    public float radius = 2;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        float value = Time.timeSinceLevelLoad * speed;
        //transform.position = new Vector2(Mathf.Sin(value) * radius, Mathf.Cos(value)*radius);
        transform.position = new Vector2(Mathf.Sin(value) * radius, Mathf.Cos(value + 2) * radius);
    }
}
