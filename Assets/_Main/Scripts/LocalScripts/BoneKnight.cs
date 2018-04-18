using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamekit2D;
using BTAI;

public class BoneKnight : MonoBehaviour, IBTDebugable {

    public Transform targetToTrack;

    
    private Animator m_Animator;
    private Rigidbody2D m_RigidBody2D;
    private SpriteRenderer m_SpriteRenderer;

    private int m_HashWalkPara = Animator.StringToHash("Walk");
    private int m_HashRunPara = Animator.StringToHash("Run");
    private int m_HashIdlePara = Animator.StringToHash("Idle");

    private Flicker m_Flicker;
    private Damageable m_Damageable;

    private Vector2 m_Force;

    //Behavior Tree
    private Root m_Ai = BT.Root();

    private void Awake()
    {
        m_Animator = GetComponentInChildren<Animator>();
        m_RigidBody2D = GetComponent<Rigidbody2D>();
        m_Damageable = GetComponent<Damageable>();
        m_SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        m_Flicker = m_Animator.gameObject.AddComponent<Flicker>();


        //Behaviour tree
        m_Ai.OpenBranch(
            BT.RandomSequence(new int[]{ 1,2}).OpenBranch(
                
                BT.Sequence().OpenBranch(
                    BT.Call(()=>m_Animator.SetBool(m_HashWalkPara,true)),
                    BT.Call(()=> SetHorizontalSpeed(1)),
                    BT.Wait(2.5f),
                    BT.Call(() => SetHorizontalSpeed(0)),
                    BT.Call(()=>m_Animator.SetBool(m_HashWalkPara, false)),
                    BT.Wait(2f)
                ),
                BT.Sequence().OpenBranch(
                    BT.Call(()=>m_Animator.SetBool(m_HashRunPara, true)),
                    BT.Call(()=> SetHorizontalSpeed(5)),
                    BT.Wait(2.5f),
                    BT.Call(()=>m_Animator.SetBool(m_HashRunPara, false)),
                    BT.Call(()=>SetHorizontalSpeed(0)),
                    BT.Wait(2f)
                )

            ),
            BT.Call(OrientToTarget),
            BT.Wait(2f)

            
        );

    }

    public Root GetAIRoot()
    {
        return m_Ai;
    }


    // Update is called once per frame
    void Update()
    {

        m_Ai.Tick();


        if(m_RigidBody2D.velocity.x < 5)
        {
            m_RigidBody2D.AddForce(m_Force);
        }

    }

    private void OrientToTarget()
    {
        if (targetToTrack == null) return;

        if (targetToTrack.position.x > transform.position.x)
        {
            m_SpriteRenderer.flipX = true;
        }
        else
        {
            m_SpriteRenderer.flipX = false;
        }

    }  


    private void SetHorizontalSpeed(int speed)
    {
        //m_RigidBody2D.velocity = speed * (!m_SpriteRenderer.flipX ? Vector2.left : Vector2.right);

        m_Force = speed * (!m_SpriteRenderer.flipX ? Vector2.left : Vector2.right);

    }


    private void Attack()
    {

    }


    public void GotHit(Damager damager, Damageable damageable)
    {
        //m_Flicker.StartFlickering(damageable.invulnerabilityDuration, timeBetweenFlickering);
    }

}
