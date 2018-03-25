using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireFlyController : MonoBehaviour {

    public GameObject bullet;
    public Transform startShootPosition;
    public GameObject abilityHolder;

    public AbilityScriptableObject[] abilities;
    public int currentAbilityIndex = 0;

    private FollowTarget m_FollowTarget;
    private SpriteRenderer m_AllessiaSpriteRenderer;
    private float m_Timer;
    private AbilityScriptableObject m_CurrentAbility = null;

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
        if (abilities.Length ==0 || currentAbilityIndex > abilities.Length-1) return;

        if (m_CurrentAbility != abilities[currentAbilityIndex])
        {
            m_CurrentAbility = abilities[currentAbilityIndex];

            //m_CurrentAbility.Initialize(abilityHolder);
        }


        //re initialize every time we trigger ability isn't effective (change later) 
        m_CurrentAbility.Initialize(abilityHolder);
        m_CurrentAbility.Trigger();

    }
    

}
