using BTAI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class rmx6BT : MonoBehaviour {
    public float thrust;
    private Root rmx6_BT = BT.Root();
    public bool targetInRange;
    private new Rigidbody2D rigidbody2D;
    private Animator animator;
    private rmx6Moving rmx6Moving;
    Vector2 oldVelocity;

	// Use this for initialization
	void Awake () {
        rigidbody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        rmx6Moving = GetComponent<rmx6Moving>();
        oldVelocity = new Vector2(0, 0);

        rmx6_BT.OpenBranch(
            BT.If(() => { return targetInRange; }).OpenBranch(

                    BT.RandomSequence().OpenBranch(
                        BT.Sequence().OpenBranch(
                        BT.Call(() => animator.SetBool("walk", true)),
                        BT.Wait(0.3f),
                        BT.Call(() => rmx6Moving.canWalk = true)
                        )
                    )
                ),
            BT.If(() => { return !targetInRange; }).OpenBranch(
                BT.RandomSequence().OpenBranch(
                    BT.Sequence().OpenBranch(
                        BT.Call(() => animator.SetBool("hide", true)),
                        BT.Call(() => animator.SetBool("walk", false)),
                        BT.Wait(0.3f),
                        BT.Call(() => rmx6Moving.canWalk = false)
                        )
                    )
                )
            );
	}
	
	// Update is called once per frame
	void Update () {
        rmx6_BT.Tick();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space press");
            rmx6_JumpToTarget();
        }
	}

    void rmx6_JumpToTarget()
    {
        Debug.Log("rmx6_jump");
        animator.SetBool("jump", true);
    }
    
    public void setVelocity()
    {
        Debug.Log("Set velocity: " + rmx6Moving.velocity);
        rmx6Moving.velocity.y = thrust;
    }

    public void ResetVelocity()
    {
        animator.SetBool("jump", false);
        Debug.Log("reset velocity : " + rmx6Moving.velocity);
        rmx6Moving.velocity.y = 0;
    }

    public void OnDie()
    {
        Debug.Log("velocity on die: " + rmx6Moving.velocity);
        rmx6Moving.velocity = new Vector2(0, 0);
        animator.SetBool("walk", false);
        animator.SetBool("freeze", true);
        rmx6Moving.canWalk = false;
        StartCoroutine(DeactiveWalk());
    }

    private IEnumerator DeactiveWalk()
    {
        yield return new WaitForSeconds(0.3f);
        rmx6Moving.canWalk = false;
    }
}
