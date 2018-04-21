using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamekit2D;
using BTAI;

public class MiniSoldier : MonoBehaviour,IBTDebugable {

    public Transform targetToTrack;
    public Damager attackDamager;

    //References
    private Animator m_Animator;
    private Rigidbody2D m_RigidBody2D;
    private SpriteRenderer m_SpriteRenderer;
    private Damageable m_Damageable;
    private Flicker m_Flicker;


    //Animations
    private int m_HashWalkPara = Animator.StringToHash("Walk");
    private int m_HashAttackPara = Animator.StringToHash("Attack");
    private int m_HashIdlePara = Animator.StringToHash("Idle");

    private Vector3 m_TargetPosition;

    //Behavior Tree
    private Root m_Ai = BT.Root();



    // Use this for initialization
    void Awake () {
        m_Animator = GetComponent<Animator>();
        m_RigidBody2D = GetComponent<Rigidbody2D>();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_Damageable = GetComponent<Damageable>();

        m_TargetPosition = targetToTrack.transform.position;

        //Behaviour tree
        m_Ai.OpenBranch(
            BT.If(() => { return (m_TargetPosition - transform.position).sqrMagnitude < 1f; }).OpenBranch(
                BT.Sequence().OpenBranch(
                    BT.Call(() => SetHorizontalSpeed(0f)),
                    BT.Call(() => m_Animator.SetTrigger(m_HashAttackPara)),
                    BT.Wait(0.5f),
                    BT.WaitForAnimatorState(m_Animator, "MiniSoldier_Idle"),
                    BT.Wait(1f)
                )
            ),
            BT.If(() => { return (m_TargetPosition - transform.position).sqrMagnitude >= 1f; }).OpenBranch(
                BT.Call(() => m_Animator.SetTrigger(m_HashWalkPara)),
                BT.Call(() => SetHorizontalSpeed(1f))
            ),
            BT.Call(() => m_TargetPosition = targetToTrack.transform.position),
            BT.Call(OrientToTarget)
        );
    }

    // Update is called once per frame
    void Update() {

        m_Ai.Tick();

    }

    public Root GetAIRoot()
    {
        return m_Ai;
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


    }

    public void StartAttack()
    {
        attackDamager.EnableDamage();

    }

    public void EndAttack()
    {
        attackDamager.DisableDamage();
    }


}
