using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class changeAnim : MonoBehaviour {

    private Animator animator;
    private bool attack;
	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();
        attack = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.X))
        {
            animator.SetBool("attack", true);
        }
	}
    public void EndAttackEvent()
    {
        //Instantiate bullet or whatsoever
        animator.SetBool("attack", false);
    }
}
