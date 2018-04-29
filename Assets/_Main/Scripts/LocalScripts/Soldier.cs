using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamekit2D;
using BTAI;

public class Soldier : MonoBehaviour, IBTDebugable {


    private SimpleEnemyBehaviour m_EnemyBehaviour;
    private Animator m_Animator;
    //private CharacterController2D m_CharacterController2D;

    private Root m_AI = BT.Root();

    public Root GetAIRoot()
    {
        return m_AI;
    }

    // Use this for initialization
    void Start () {
        //m_CharacterController2D = GetComponent<CharacterController2D>()l
        m_Animator = GetComponent<Animator>();
        m_EnemyBehaviour = GetComponent<SimpleEnemyBehaviour>();

        m_AI.OpenBranch(

           BT.If(() => { return m_EnemyBehaviour.CurrentTarget != null; }).OpenBranch(
               BT.Call(m_EnemyBehaviour.CheckTargetStillVisible),
               //Run to target
               BT.If(()=> { return !m_EnemyBehaviour.CheckMeleeAttack(); }).OpenBranch(
                   BT.Call(()=>m_EnemyBehaviour.RunToTarget(0.2f))
               ),
               //Attack
               BT.If(() => { return m_EnemyBehaviour.CheckMeleeAttack(); }).OpenBranch(
                   BT.Call(m_EnemyBehaviour.StopPatrolling),
                   BT.Call(m_EnemyBehaviour.StopRunningToTarget),
                   BT.Call(m_EnemyBehaviour.PerformMeleeAttack),
                   BT.Wait(0.5f),
                   BT.WaitForAnimatorState(m_Animator, "MiniSoldier_Idle")
               )
           ),

           //Patrolling Around
           BT.If(() => { return m_EnemyBehaviour.CurrentTarget == null; }).OpenBranch(
               BT.Call(m_EnemyBehaviour.ScanForTarget),
               BT.Call(()=>m_EnemyBehaviour.Patrolling(0.2f))
           )
       );

    }
	
    

	// Update is called once per frame
	void Update () {


        m_AI.Tick();

	}






}
