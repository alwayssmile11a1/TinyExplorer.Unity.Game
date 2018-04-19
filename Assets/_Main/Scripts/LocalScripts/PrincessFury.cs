using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamekit2D;
using BTAI;

public class PrincessFury : MonoBehaviour, IBTDebugable {

    public Transform targetToTrack;

    public Damager shortDamager;
    public Damager longDamager;
    public Damager jumpDamager;
    
    private Animator m_Animator;
    private Rigidbody2D m_RigidBody2D;
    private SpriteRenderer m_SpriteRenderer;

    private int m_WakeUpPara = Animator.StringToHash("WakeUp");
    private int m_HashWalkPara = Animator.StringToHash("Walk");
    private int m_HashTopAttackPara = Animator.StringToHash("TopAttack");
    private int m_HashBottomAttackPara = Animator.StringToHash("BottomAttack");
    private int m_HashTopToDowmAttackPara = Animator.StringToHash("TopToDownAttack");
    private int m_HashJumpAttackPara = Animator.StringToHash("JumpAttack");
    private int m_HashIdlePara = Animator.StringToHash("Idle");


    private Flicker m_Flicker;
    private Damageable m_Damageable;

    private Vector2 m_Force;

    private bool m_WokeUp = false;


    //Behavior Tree
    private Root m_Ai = BT.Root();

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
        m_RigidBody2D = GetComponent<Rigidbody2D>();
        m_Damageable = GetComponent<Damageable>();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_Flicker = gameObject.AddComponent<Flicker>();


        shortDamager.DisableDamage();
        longDamager.DisableDamage();
        jumpDamager.DisableDamage();

        WakeUp();

        //Behaviour tree
        m_Ai.OpenBranch(
            BT.If(() => { return m_WokeUp; }).OpenBranch(

                BT.RandomSequence(new int[] { 1, 1 }).OpenBranch(
                    BT.Sequence().OpenBranch(
                        BT.Call(() => m_Animator.SetBool(m_HashWalkPara, true)),
                        BT.Call(() => SetHorizontalSpeed(1f)),
                        BT.Wait(2.5f),
                        BT.Call(() => SetHorizontalSpeed(0)),
                        BT.Call(() => m_Animator.SetBool(m_HashWalkPara, false)),
                        BT.Wait(1f)
                    ),
                    BT.Sequence().OpenBranch(
                        BT.Call(() => m_Animator.SetTrigger(m_HashTopAttackPara)),
                        BT.WaitForAnimatorState(m_Animator, "PrincessFury_Idle"),
                        BT.Call(() => longDamager.DisableDamage()),
                        BT.Wait(1f)
                    ),
                    BT.Sequence().OpenBranch(
                        BT.Call(() => m_Animator.SetTrigger(m_HashBottomAttackPara)),
                        BT.WaitForAnimatorState(m_Animator, "PrincessFury_Idle"),
                        BT.Call(() => shortDamager.DisableDamage()),
                        BT.Wait(1f)
                    ),
                    BT.Sequence().OpenBranch(
                        BT.Call(() => m_Animator.SetTrigger(m_HashJumpAttackPara)),
                        BT.WaitForAnimatorState(m_Animator, "PrincessFury_Idle"),
                        BT.Call(() => jumpDamager.DisableDamage()),
                        BT.Wait(1f)
                    ),
                    BT.Sequence().OpenBranch(
                        BT.Call(() => m_Animator.SetTrigger(m_HashTopToDowmAttackPara)),
                        BT.WaitForAnimatorState(m_Animator, "PrincessFury_Idle"),
                        BT.Call(() => shortDamager.DisableDamage()),
                        BT.Wait(1f)
                    )

                ),

                BT.Call(OrientToTarget),

                BT.Wait(1f)

            )


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
            m_SpriteRenderer.flipX = false;
        }
        else
        {
            m_SpriteRenderer.flipX = true;
        }

    }  


    private void SetHorizontalSpeed(float speed)
    {
        m_RigidBody2D.velocity = speed * (m_SpriteRenderer.flipX ? Vector2.left : Vector2.right);

        //m_Force = speed * (!m_SpriteRenderer.flipX ? Vector2.left : Vector2.right);

    }

    public void WakeUp()
    {
        if (m_WokeUp == false)
        {
            m_Animator.SetTrigger(m_WakeUpPara);
        }

        m_WokeUp = true;

    }

    public void TopAttack()
    {
        longDamager.EnableDamage();
    }


    public void BottomAttack()
    {
        shortDamager.EnableDamage();
    }

    public void JumpAttack()
    {
        jumpDamager.EnableDamage();
    }




    public void GotHit(Damager damager, Damageable damageable)
    {
        //m_Flicker.StartFlickering(damageable.invulnerabilityDuration, timeBetweenFlickering);
    }

}
