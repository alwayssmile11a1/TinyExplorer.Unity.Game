using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamekit2D;
using BTAI;

public class MiniSoldier : MonoBehaviour,IBTDebugable {

    public Damager bodyDamager;
    public Damager attackDamager;


    //References
    private Transform m_TargetToTrack;
    private Animator m_Animator;
    private Rigidbody2D m_RigidBody2D;
    private SpriteRenderer m_SpriteRenderer;
    //private Damageable m_Damageable;


    //Animations
    private int m_HashWalkPara = Animator.StringToHash("Walk");
    private int m_HashAttackPara = Animator.StringToHash("Attack");
    //private int m_HashIdlePara = Animator.StringToHash("Idle");

    private bool actived = false;

    private Vector3 m_TargetPosition;

    //Behavior Tree
    private Root m_Ai = BT.Root();



    // Use this for initialization
    void Awake() {

        m_TargetToTrack = GameObject.FindGameObjectWithTag("Player").transform;

        m_Animator = GetComponent<Animator>();
        m_RigidBody2D = GetComponent<Rigidbody2D>();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        //m_Damageable = GetComponent<Damageable>();
        
        m_TargetPosition = m_TargetToTrack.transform.position;

        //Behaviour tree
        m_Ai.OpenBranch(
            BT.If(() => { return (m_TargetPosition - transform.position).sqrMagnitude < 0.9f; }).OpenBranch(
                BT.Call(() => m_Animator.ResetTrigger(m_HashWalkPara)),
                BT.Call(() => SetHorizontalSpeed(0f)),
                BT.Call(() => m_Animator.SetTrigger(m_HashAttackPara)),
                BT.Wait(0.5f),
                BT.WaitForAnimatorState(m_Animator, "MiniSoldier_Idle")
            ),
            BT.If(() => { return (m_TargetPosition - transform.position).sqrMagnitude >= 0.9f; }).OpenBranch(
                BT.Call(() => m_Animator.SetTrigger(m_HashWalkPara)),
                BT.Call(() => SetHorizontalSpeed(1f))
            ),
            BT.Call(() => m_TargetPosition = m_TargetToTrack.transform.position),
            BT.Wait(0.5f),
            BT.Call(OrientToTarget)
        );
    }

    // Update is called once per frame
    void Update() {

        if (actived)
        {
            m_Ai.Tick();
        }
    }

    public void Active()
    {

        m_SpriteRenderer.sortingOrder = 5;

        Color color = m_SpriteRenderer.color;
        color.a = 1f;
        m_SpriteRenderer.color = color;

        bodyDamager.EnableDamage();

        OrientToTarget();

        actived = true;
    }

    public void DeActive()
    {
        m_SpriteRenderer.sortingOrder = 0;

        Color color = m_SpriteRenderer.color;
        color.a = 0.8f;
        m_SpriteRenderer.color = color;

        bodyDamager.DisableDamage();

        actived = false;
    }

    public Root GetAIRoot()
    {
        return m_Ai;
    }

    private void OrientToTarget()
    {
        if (m_TargetToTrack == null) return;

        if (m_TargetToTrack.position.x > transform.position.x)
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

        speed += Random.Range(-0.2f, 0.2f);

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
