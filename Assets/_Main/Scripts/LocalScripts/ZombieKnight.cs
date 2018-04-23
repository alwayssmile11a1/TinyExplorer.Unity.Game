using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Gamekit2D;
using BTAI;

public class ZombieKnight : MonoBehaviour, IBTDebugable{

    public Transform targetToTrack;

    [Header("Damagers")]
    public Damager shortDamager;



    //Animation
    private int m_HashIdle1Para = Animator.StringToHash("Idle");
    private int m_HashIdle2Para = Animator.StringToHash("Idle2");
    private int m_HashWalkPara = Animator.StringToHash("Walk");
    private int m_HashRunPara = Animator.StringToHash("Run");
    private int m_HashDeathPara = Animator.StringToHash("Death");
    private int m_HashSkill1Para = Animator.StringToHash("Skill1");
    private int m_HashSkill2Para = Animator.StringToHash("Skill2");

    //References
    private Animator m_Animator;
    private Rigidbody2D m_RigidBody2D;
    private Damageable m_Damageable;
    private Flicker m_Flicker;
    private Vector3 m_OriginalLocalScale;

    //Behavior Tree
    private Root m_Ai = BT.Root();


    private void Awake()
    {
        m_Animator = GetComponentInChildren<Animator>();
        m_RigidBody2D = GetComponent<Rigidbody2D>();
        m_Damageable = GetComponent<Damageable>();


        m_OriginalLocalScale = transform.localScale;
        //m_Flicker = gameObject.AddComponent<Flicker>();


        //Behaviour tree
        m_Ai.OpenBranch(

            BT.RandomSequence(new int[] { 2, 1, 2, 2, 2, 1 }, 2).OpenBranch(
                //Walk
                BT.Sequence().OpenBranch(
                    BT.Call(() => m_Animator.SetBool(m_HashWalkPara, true)),
                    BT.Call(() => SetHorizontalSpeed(1f)),
                    BT.Wait(2.5f),
                    BT.Call(() => SetHorizontalSpeed(0)),
                    BT.Call(() => m_Animator.SetBool(m_HashWalkPara, false)),
                    BT.Wait(0.5f)
                ),
                //Run
                BT.Sequence().OpenBranch(
                    BT.Call(() => m_Animator.SetBool(m_HashRunPara, true)),
                    BT.Call(() => SetHorizontalSpeed(3f)),
                    BT.Wait(2.5f),
                    BT.Call(() => SetHorizontalSpeed(0)),
                    BT.Call(() => m_Animator.SetBool(m_HashRunPara, false)),
                    BT.Wait(0.5f)
                ),
                //Skill1
                BT.Sequence().OpenBranch(
                    BT.Call(() => m_Animator.SetTrigger(m_HashSkill1Para)),
                    BT.Wait(0.5f),
                    BT.WaitForAnimatorState(m_Animator,"idle_1"),
                    BT.Wait(1f)
                ),
                //Skill2
                BT.Sequence().OpenBranch(
                    BT.Call(() => m_Animator.SetTrigger(m_HashSkill2Para)),
                    BT.Wait(0.5f),
                    BT.WaitForAnimatorState(m_Animator, "idle_1"),
                    BT.Wait(1f)
                )

            ),

            BT.Call(OrientToTarget),

            BT.Wait(1f)



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

    }

    private void OrientToTarget()
    {
        if (targetToTrack == null) return;

        if (targetToTrack.position.x < transform.position.x)
        {
            transform.localScale = m_OriginalLocalScale;
        }
        else
        {
            Vector3 reverseScale = m_OriginalLocalScale;
            reverseScale.x *= -1;
            transform.localScale = reverseScale;
        }
    }


    private void SetHorizontalSpeed(float speed)
    {
        m_RigidBody2D.velocity = speed * (transform.lossyScale.x > 0 ? Vector2.left : Vector2.right);

    }


    public void GotHit(Damager damager, Damageable damageable)
    {
        m_Flicker.StartColorFickering(damageable.invulnerabilityDuration, 0.1f);
    }

}
