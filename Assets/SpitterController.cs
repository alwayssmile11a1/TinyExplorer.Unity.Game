using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpitterController : MonoBehaviour {

    public GameObject bullet;
    public Transform startShootPosition;
    public float fireRate = 5f;
    public float displayTime = 1f;

    private SpriteRenderer m_spriteRenderer;
    private float timer;
    private Animator m_Animator;

	// Use this for initialization
	void Awake () {
        m_spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        m_Animator = GetComponentInChildren<Animator>();

    }
	
	// Update is called once per frame
	void Update () {
		if(timer>0)
        {
            timer -= Time.deltaTime;
            
        }
        else
        {
            //m_Animator.SetTrigger("Disappear");
        }
	}


    public void Trigger(Vector3 position)
    {
        if (timer < 0)
        {
            transform.position = position;
        }

        GameObject cloneBullet = Instantiate(bullet, startShootPosition.position, startShootPosition.rotation);
        //cloneBullet.GetComponent<Rigidbody2D>().velocity = 5f;

        Destroy(cloneBullet, 2f);
        
        timer = displayTime;
        m_spriteRenderer.enabled = true;
        m_Animator.SetTrigger("Spotted");
        m_Animator.SetTrigger("Shooting");

    }


}
