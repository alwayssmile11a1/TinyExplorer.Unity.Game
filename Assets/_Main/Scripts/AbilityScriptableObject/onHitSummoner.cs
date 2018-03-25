using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class onHitSummoner : MonoBehaviour {
    public GameObject hitEffect;
    [Tooltip("Just testing")]
    public Transform particlePos;
	void OnTriggerEnter(Collider collider)
    {
        //if (collider.tag.Equals("Summoner"))
        int a = 2;
        {
            Debug.Log("hit: " + collider.tag);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag =="Acid")
        {
            Debug.Log("hit: " );
            Instantiate(hitEffect, particlePos);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }

}
