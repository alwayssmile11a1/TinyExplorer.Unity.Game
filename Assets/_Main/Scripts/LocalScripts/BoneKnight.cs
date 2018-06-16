using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BTAI;
using Gamekit2D;

public class BoneKnight : MonoBehaviour {

    public Damager damager;

    private SimpleEnemyBehaviour m_EnemyBehaviour;
    private Animator m_Animator;

    private Root m_AI = BT.Root();

    private int m_HashAttack = VFXController.StringToHash("SmallImplode");

    private Rigidbody2D m_RigidBody2D;


    // Use this for initialization
    void Awake()
    {
        m_Animator = GetComponent<Animator>();
        m_EnemyBehaviour = GetComponentInParent<SimpleEnemyBehaviour>();
        m_RigidBody2D = GetComponentInParent<Rigidbody2D>();

        m_AI.OpenBranch(
           BT.If(() => { return m_EnemyBehaviour.CurrentTarget != null; }).OpenBranch(
               BT.Call(m_EnemyBehaviour.CheckTargetStillVisible),
               //BT.If(() => { return !m_EnemyBehaviour.CheckMeleeAttack(); }).OpenBranch(
               //    BT.If(() => { return !m_EnemyBehaviour.CheckForObstacle(0.3f); }).OpenBranch(
               //        BT.Call(() => m_EnemyBehaviour.RunToTarget(0.2f)),
               //        BT.WaitUntil(() => { return m_EnemyBehaviour.CheckForObstacle(0.3f); }),
               //        BT.Call(m_EnemyBehaviour.StopRunning)
               //    )
               //),
               //Attack
               //BT.If(() => { return m_EnemyBehaviour.CheckMeleeAttack(); }).OpenBranch(
                   BT.Call(m_EnemyBehaviour.OrientToTarget),
                   BT.Call(m_EnemyBehaviour.StopPatrolling),
                   BT.Call(() => m_Animator.SetTrigger(m_EnemyBehaviour.HashMeleeAttackPara)),
                   BT.Wait(0.5f),
                   BT.WaitForAnimatorState(m_Animator, "BoneKnight_Idle"),
                   BT.Wait(1f),
                   BT.Call(m_EnemyBehaviour.ForgetTarget)
               //)
           ),

           //Patrolling Around
           BT.If(() => { return m_EnemyBehaviour.CurrentTarget == null; }).OpenBranch(
               BT.Call(m_EnemyBehaviour.ScanForTarget),
               BT.Call(() => m_EnemyBehaviour.Patrolling(0.3f))
           )
            
       );

    }



    // Update is called once per frame
    void Update()
    {


        m_AI.Tick();

    }


    public Root GetAIRoot()
    {
        return m_AI;
    }

    public void StartAttacking()
    {
        if(m_EnemyBehaviour.GetForwardVector().x > 0)
        {
            VFXController.Instance.Trigger(m_HashAttack, transform.position + new Vector3(-0.1f, 0.58f, 0), 0, false, null);
        }
        else
        {
            VFXController.Instance.Trigger(m_HashAttack, transform.position + new Vector3(0.1f, 0.58f, 0), 0, false, null);
        }
    }


    public void Attack()
    {
        damager.EnableDamage();

        m_EnemyBehaviour.OrientToTarget();
        Vector2 direction = (Vector2)m_EnemyBehaviour.targetToTrack.transform.position - m_RigidBody2D.position;
        direction.y = 0;

        if(!m_EnemyBehaviour.CheckForObstacle(Mathf.Abs(direction.x * 0.8f)))
        {
            m_RigidBody2D.MovePosition(direction * 0.8f + m_RigidBody2D.position);
        }
        PlaySlashAudio();
    }

    public void EndAttack()
    {
        damager.DisableDamage();
    }


    public void PlayFootStep()
    {
        m_EnemyBehaviour.PlayFootStep();
    }

    public void PlaySlashAudio()
    {
        m_EnemyBehaviour.PlayMeleeAttackAudio();
    }

}
