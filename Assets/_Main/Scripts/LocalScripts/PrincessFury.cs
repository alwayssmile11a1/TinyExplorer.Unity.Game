using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamekit2D;
using BTAI;

public class PrincessFury : MonoBehaviour, IBTDebugable {

    public Transform targetToTrack;

    [Header("Damagers")]
    public Damager shortDamager;
    public Damager longDamager;
    public Damager jumpDamager;


    [Header("Spells")]
    public GameObject darkBallSpell;
    public GameObject jumpAttackSpell;
    

    //Animation
    private int m_WakeUpPara = Animator.StringToHash("WakeUp");
    private int m_HashWalkPara = Animator.StringToHash("Walk");
    private int m_HashTopAttackPara = Animator.StringToHash("TopAttack");
    private int m_HashBottomAttackPara = Animator.StringToHash("BottomAttack");
    private int m_HashTopToDowmAttackPara = Animator.StringToHash("TopToDownAttack");
    private int m_HashJumpAttackPara = Animator.StringToHash("JumpAttack");
    private int m_HashIdlePara = Animator.StringToHash("Idle");
    private int m_HashSummonPara = Animator.StringToHash("Summon");
    private int m_HashDeathPara = Animator.StringToHash("Death");

    //References
    private Animator m_Animator;
    private Rigidbody2D m_RigidBody2D;
    private SpriteRenderer m_SpriteRenderer;
    private Damageable m_Damageable;
    private Flicker m_Flicker;


    //Other variables
    private bool m_WokeUp = false;
    private BulletPool m_JumpAttackSpellPool;
    private BulletPool m_DarkMatterPool;

    private List<BulletObject> m_DarkMatters;
    private float m_DarkMatterTimer;

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


        m_JumpAttackSpellPool = BulletPool.GetObjectPool(jumpAttackSpell, 4);
        m_DarkMatterPool = BulletPool.GetObjectPool(darkBallSpell, 1);
        m_DarkMatters = m_DarkMatterPool.GetAll();

        WakeUp();
        


        //Behaviour tree
        m_Ai.OpenBranch(

            //Death
            BT.If(() => { return m_Damageable.CurrentHealth <= 0; }).OpenBranch(
                BT.Call(()=>m_Animator.SetTrigger(m_HashDeathPara))
            ),

            //Still Alive
            BT.If(() => { return m_Damageable.CurrentHealth > 0; }).OpenBranch(
                //Woke up
                BT.If(() => { return m_WokeUp; }).OpenBranch(
                    BT.RandomSequence(new int[] { 1, 3 }).OpenBranch(
                        ////Walk
                        //BT.Sequence().OpenBranch(
                        //    BT.Call(() => m_Animator.SetBool(m_HashWalkPara, true)),
                        //    BT.Call(() => SetHorizontalSpeed(1f)),
                        //    BT.Wait(2.5f),
                        //    BT.Call(() => SetHorizontalSpeed(0)),
                        //    BT.Call(() => m_Animator.SetBool(m_HashWalkPara, false)),
                        //    BT.Wait(1f)
                        //),
                        //Top Attack
                        BT.Sequence().OpenBranch(
                            BT.Call(() => m_Animator.SetTrigger(m_HashTopAttackPara)),
                            BT.WaitForAnimatorState(m_Animator, "PrincessFury_Idle"),
                            BT.Wait(5f)
                        )
                        // BT.Sequence().OpenBranch(
                        //    BT.Call(() => m_Animator.SetTrigger(m_HashBottomAttackPara)),
                        //    BT.Wait(0.5f),
                        //    BT.WaitForAnimatorState(m_Animator, "PrincessFury_Idle"),
                        //    BT.Call(()=>m_Animator.SetTrigger(m_HashTopToDowmAttackPara)),
                        //    BT.Wait(0.5f),
                        //    BT.WaitForAnimatorState(m_Animator, "PrincessFury_Idle"),
                        //    BT.Call(() => m_Animator.SetTrigger(m_HashBottomAttackPara)),
                        //    BT.Wait(0.5f),
                        //    BT.WaitForAnimatorState(m_Animator, "PrincessFury_Idle"),
                        //    BT.Wait(2f)
                        //),
                        //BT.Sequence().OpenBranch(
                        //    BT.Call(() => m_Animator.SetTrigger(m_HashJumpAttackPara)),
                        //    BT.WaitForAnimatorState(m_Animator, "PrincessFury_Idle"),
                        //    BT.Wait(2f)
                        //)
                        //BT.Sequence().OpenBranch(
                        //    BT.Call(() => m_Animator.SetTrigger(m_HashTopToDowmAttackPara)),
                        //    BT.WaitForAnimatorState(m_Animator, "PrincessFury_Idle"),
                        //    BT.Call(() => shortDamager.DisableDamage()),
                        //    BT.Wait(1f)
                        //)

                    ),

                    BT.Call(OrientToTarget),

                    BT.Wait(1f)

                )
               
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


        if (!m_DarkMatters[0].inPool )
        {
            m_DarkMatters[0].rigidbody2D.position = new Vector2(m_DarkMatters[0].rigidbody2D.position.x, transform.position.y + Mathf.Sin(3 * m_DarkMatterTimer)*1.5f);
            m_DarkMatters[1].rigidbody2D.position = new Vector2(m_DarkMatters[1].rigidbody2D.position.x, transform.position.y - Mathf.Sin(3 * m_DarkMatterTimer)*1.5f);
            m_DarkMatterTimer += Time.deltaTime;
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

    public void StartTopAttack()
    {
        longDamager.EnableDamage();

        BulletObject darkMatter1 = m_DarkMatterPool.Pop(transform.position + (m_SpriteRenderer.flipX? new Vector3(-0.5f, 0, 0): new Vector3(0.5f,0,0)));
        darkMatter1.rigidbody2D.velocity = (m_SpriteRenderer.flipX ? Vector2.left : Vector2.right) * 2;

        BulletObject darkMatter2 = m_DarkMatterPool.Pop(transform.position + (m_SpriteRenderer.flipX ? new Vector3(-0.5f, 0, 0) : new Vector3(0.5f, 0, 0)));
        darkMatter2.rigidbody2D.velocity = (m_SpriteRenderer.flipX ? Vector2.left : Vector2.right) * 2;

        m_DarkMatterTimer = 0;

    }

    public void EndTopAttack()
    {
        longDamager.DisableDamage();
    }

    public void StartBottomAttack()
    {
        shortDamager.EnableDamage();
    }

    public void EndBottomBottomAttack()
    {
        shortDamager.DisableDamage();
    }

    public void StartJumpAttack()
    {
        jumpDamager.EnableDamage();

        //Fire magic spells
        for (int i = 0; i < 4; i++)
        {
            Vector3 rightOffset = new Vector3(1.5f - i*0.2f, (i - 1) * 0.35f + 0.1f, 0);
            Vector3 leftOffset = rightOffset;
            leftOffset.x *= -1;

            if (m_SpriteRenderer.flipX)
            {
                BulletObject bulletObject = m_JumpAttackSpellPool.Pop(transform.position + leftOffset );
                bulletObject.rigidbody2D.velocity = Vector2.left  * 7;
                bulletObject.rigidbody2D.transform.RotateToDirection(Vector2.right);
            }
            else
            {
                BulletObject bulletObject = m_JumpAttackSpellPool.Pop(transform.position + rightOffset);
                bulletObject.rigidbody2D.velocity = Vector2.right * 7;
                bulletObject.rigidbody2D.transform.RotateToDirection(Vector2.left);
            }
        }

    }

    public void EndJumpAttack()
    {
        jumpDamager.DisableDamage();
    }





    public void GotHit(Damager damager, Damageable damageable)
    {
        //m_Flicker.StartFlickering(damageable.invulnerabilityDuration, timeBetweenFlickering);
    }

}
