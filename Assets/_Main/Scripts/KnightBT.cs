using BTAI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightBT : MonoBehaviour {
    Animator m_Animator;
    Root m_Ai = BT.Root();
    EnemyBehaviour m_EnemyBehaviour;
    // Use this for initialization
    private void OnEnable()
    {
        m_EnemyBehaviour = GetComponent<EnemyBehaviour>();
        m_Animator = GetComponent<Animator>();

        m_Ai.OpenBranch(

            BT.If(() => { return m_EnemyBehaviour.CurrentTarget != null; }).OpenBranch(
                BT.Call(m_EnemyBehaviour.CheckTargetStillVisible),
                BT.Call(m_EnemyBehaviour.OrientToTarget),
                BT.Trigger(m_Animator, "attack"),
                BT.Call(m_EnemyBehaviour.RememberTargetPos),
                BT.WaitForAnimatorState(m_Animator, "rideloid_attack")
            ),

            BT.If(() => { return m_EnemyBehaviour.CurrentTarget == null; }).OpenBranch(
                BT.Call(m_EnemyBehaviour.ScanForTarget)
            )
        );
    }

    // Update is called once per frame
    void Update () {
        m_Ai.Tick();
	}
}
