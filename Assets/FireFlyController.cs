using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireFlyController : MonoBehaviour {

    public GameObject bullet;
    public Transform startShootPosition;
    public float fireRate = 5f;
    public float timeToReturnOriginalPosition = 2f;

    private FollowTarget m_FollowTarget;
    private SpriteRenderer m_AllessiaSpriteRenderer;
    private float m_Timer;

    // Use this for initialization
    void Awake()
    {
        m_FollowTarget = GetComponent<FollowTarget>();
        m_AllessiaSpriteRenderer = m_FollowTarget.target.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(m_Timer>0)
        {
            m_Timer -= Time.deltaTime;


        }
        else
        {
            m_FollowTarget.usedOffset = 0;
        }




    }


    //new offset from player of the firefly
    public void Trigger()
    {

        Quaternion eulerRotation = startShootPosition.rotation;

        if(m_AllessiaSpriteRenderer.flipX)
        {
            eulerRotation = startShootPosition.rotation * Quaternion.Euler(0, 0, 180);
        }

        GameObject cloneBullet = Instantiate(bullet, startShootPosition.position, eulerRotation);
        //cloneBullet.GetComponent<Rigidbody2D>().velocity = 5f;

        Destroy(cloneBullet, 2f);

        m_Timer = timeToReturnOriginalPosition;

        m_FollowTarget.usedOffset = 1;


    }




}
