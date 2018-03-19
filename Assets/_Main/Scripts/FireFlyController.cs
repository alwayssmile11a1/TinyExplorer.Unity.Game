using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireFlyController : MonoBehaviour {

    public GameObject bullet;
    public Transform startShootPosition;
    public AbilitySO currentAbility;
    public GameObject abilityTriggerableHolder;

    private FollowTarget m_FollowTarget;
    private SpriteRenderer m_AllessiaSpriteRenderer;
    private float m_Timer;
    //private AbilitySO m_CurrentAbility = null;

    // Use this for initialization
    private void Awake()
    {
        m_FollowTarget = GetComponent<FollowTarget>();
        m_AllessiaSpriteRenderer = m_FollowTarget.target.GetComponent<SpriteRenderer>();
        

    }

    // Update is called once per frame
    private void Update()
    {


        //Update transform rotation
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mousePosition - transform.position;
        float zDegree = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0,zDegree);
 
    }


    //new offset from player of the firefly
    public void TriggerAbility()
    {

        currentAbility.Initialize(abilityTriggerableHolder);
        currentAbility.Trigger();

    }
    

}
