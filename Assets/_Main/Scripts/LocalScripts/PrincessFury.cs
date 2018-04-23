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

    public GameObject concentratingEffect;


    [Header("Summon")]
    public Transform summonPosition1;
    public Transform summonPosition2;
    public Transform summonTeleportPosition;
    public GameObject miniSoldier;

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
    private BulletObject m_DarkMatter1;
    private BulletObject m_DarkMatter2;
    private float m_DarkMatterTimer;
    private Vector3 m_PrincessFuryPosition;


    private BulletPool m_SoldierPool;
    private List<BulletObject> m_Soldiers = new List<BulletObject>();
    private int m_SoldierCount = 3;
    private float m_SoldierAppearingTimer;

    //Teleport
    private float m_TeleportTimer;
    private Vector3 m_TeleportToPosition;

    private int m_VortexGroundEffectHash = VFXController.StringToHash("Vortex_Ground");

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
        m_DarkMatterPool = BulletPool.GetObjectPool(darkBallSpell, 2);
        m_SoldierPool = BulletPool.GetObjectPool(miniSoldier, 4);

        WakeUp();



        //Behaviour tree
        m_Ai.OpenBranch(

            //Death
            BT.If(() => { return m_Damageable.CurrentHealth <= 0; }).OpenBranch(
                BT.Call(() => m_Animator.SetTrigger(m_HashDeathPara))
            ),

            //Still Alive
            BT.If(() => { return m_Damageable.CurrentHealth > 0; }).OpenBranch(
                //Woke up
                BT.If(() => { return m_WokeUp; }).OpenBranch(
                    BT.RandomSequence(new int[] { 2, 1, 2, 2, 2,1 }, 2).OpenBranch(
                        //Walk
                        BT.Sequence().OpenBranch(
                            BT.Call(() => m_Animator.SetBool(m_HashWalkPara, true)),
                            BT.Call(() => SetHorizontalSpeed(1f)),
                            BT.Wait(2.5f),
                            BT.Call(() => SetHorizontalSpeed(0)),
                            BT.Call(() => m_Animator.SetBool(m_HashWalkPara, false)),
                            BT.Wait(0.5f)
                        ),
                        //Teleport
                        BT.Sequence().OpenBranch(
                            BT.Call(() => TeleportToRandom()),
                            BT.Wait(4.5f)
                        ),
                        //Top to bottom Attack
                        BT.Sequence().OpenBranch(
                            BT.Call(() => SetConcetratingEffectLocalPosition(new Vector2(-1.3f, 1f))),
                            BT.Call(() => m_Animator.SetTrigger(m_HashTopToDowmAttackPara)),
                            BT.Wait(0.5f),
                            BT.WaitForAnimatorState(m_Animator, "PrincessFury_Idle"),
                            BT.Wait(1.5f)
                        ),
                         //Combo Attack
                         BT.If(() => { return (targetToTrack.position - transform.position).sqrMagnitude < 4f; }).OpenBranch(
                             BT.Sequence().OpenBranch(
                                BT.Call(() => m_Animator.SetTrigger(m_HashTopAttackPara)),
                                BT.Wait(0.5f),
                                BT.WaitForAnimatorState(m_Animator, "PrincessFury_Idle"),
                                BT.Call(OrientToTarget),
                                BT.Call(() => m_Animator.SetTrigger(m_HashBottomAttackPara)),
                                BT.Wait(0.5f),
                                BT.WaitForAnimatorState(m_Animator, "PrincessFury_Idle"),
                                BT.Call(OrientToTarget),
                                BT.Call(() => m_Animator.SetTrigger(m_HashTopAttackPara)),
                                BT.Wait(0.5f),
                                BT.WaitForAnimatorState(m_Animator, "PrincessFury_Idle"),
                                BT.Wait(1f)
                            )
                        ),
                        //JumpAttack
                        BT.Sequence().OpenBranch(
                            BT.Call(() => SetConcetratingEffectLocalPosition(new Vector2(-0.85f, -0.75f))),
                            BT.Call(() => m_Animator.SetTrigger(m_HashJumpAttackPara)),
                            BT.WaitForAnimatorState(m_Animator, "PrincessFury_Idle"),
                            BT.Wait(1.5f)
                        ),
                        //Summon
                        BT.Sequence().OpenBranch(
                            BT.Call(() => TeleportTo(summonTeleportPosition.position)),
                            BT.Wait(4.5f),
                            BT.Call(() => m_SpriteRenderer.flipX = true),
                            BT.Call(() => SetConcetratingEffectLocalPosition(new Vector2(-0.15f, 1.5f))),
                            BT.Call(() => m_Animator.SetTrigger(m_HashSummonPara)),
                            BT.Wait(1f),
                            BT.WaitForAnimatorState(m_Animator, "PrincessFury_Idle"),
                            BT.WaitUntil(SummonActionEndCheck),                           
                            BT.Wait(0.5f),
                            BT.Call(() => TeleportToRandom()),
                            BT.Wait(5f)
                        )


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


        if (m_DarkMatter1 !=null && !m_DarkMatter1.inPool)
        {
            m_DarkMatter1.rigidbody2D.position = new Vector2(m_DarkMatter1.rigidbody2D.position.x, m_PrincessFuryPosition.y + Mathf.Sin(3 * m_DarkMatterTimer)*1.5f);
            m_DarkMatter2.rigidbody2D.position = new Vector2(m_DarkMatter2.rigidbody2D.position.x, m_PrincessFuryPosition.y - Mathf.Sin(3 * m_DarkMatterTimer)*1.5f);
            m_DarkMatterTimer += Time.deltaTime;
        }


        if(m_SoldierAppearingTimer>0)
        {
            m_SoldierAppearingTimer -= Time.deltaTime;

            if(m_SoldierAppearingTimer<=0)
            {
                for (int i = 0; i < m_Soldiers.Count; i++)
                {
                    m_Soldiers[i].rigidbody2D.velocity = Vector2.zero;
                    m_Soldiers[i].transform.GetComponent<MiniSoldier>().Active();
                }


            }

        }


        if (m_TeleportTimer > 0)
        {
            m_TeleportTimer -= Time.deltaTime;

            if (m_TeleportTimer <= 0)
            {
                m_RigidBody2D.velocity = Vector2.zero;
                m_SpriteRenderer.sortingOrder = 5;
            }
            else
            {
                if (m_TeleportTimer <= 2f && m_RigidBody2D.velocity.y < 0)
                {
                    m_RigidBody2D.position = new Vector2(m_TeleportToPosition.x, m_TeleportToPosition.y - 1f);
                    m_RigidBody2D.velocity = Vector2.up * 0.75f;
                }
            }

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

    }

    public void EndTopAttack()
    {
        longDamager.DisableDamage();
    }

    public void StartTopToBottomAttack()
    {
        shortDamager.EnableDamage();

        m_DarkMatter1 = m_DarkMatterPool.Pop(transform.position + (m_SpriteRenderer.flipX ? new Vector3(-0.5f, 0, 0) : new Vector3(0.5f, 0, 0)));
        m_DarkMatter1.rigidbody2D.velocity = (m_SpriteRenderer.flipX ? Vector2.left : Vector2.right) * 2;

        m_DarkMatter2 = m_DarkMatterPool.Pop(transform.position + (m_SpriteRenderer.flipX ? new Vector3(-0.5f, 0, 0) : new Vector3(0.5f, 0, 0)));
        m_DarkMatter2.rigidbody2D.velocity = (m_SpriteRenderer.flipX ? Vector2.left : Vector2.right) * 2;

        m_PrincessFuryPosition = transform.position;

        m_DarkMatterTimer = 0;

    }

    public void EndTopToBottomAttack()
    {
        shortDamager.DisableDamage();
    }



    public void StartBottomAttack()
    {
        shortDamager.EnableDamage();
    }

    public void EndBottomAttack()
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
    
    public void SetConcetratingEffectLocalPosition(Vector2 localPosition)
    {
        concentratingEffect.transform.localPosition = localPosition;

        //Set offset of concentrating effect based on sprite facing
        concentratingEffect.transform.ChangeOffsetBasedOnSpriteFacing(transform, m_SpriteRenderer, localPosition);

    }

    public void EnableConcentraingEffect()
    {
        concentratingEffect.SetActive(true);
    }

    public void DisableConcentratingEffect()
    {
        concentratingEffect.SetActive(false);
    }

    
    public void SummonMiniSoldier()
    {
        m_Soldiers.Clear();

        for (int i = 0; i < m_SoldierCount; i++)
        {
            float xPosition = Random.Range(summonPosition1.position.x, summonPosition2.position.x);
            VFXController.Instance.Trigger(m_VortexGroundEffectHash, new Vector3(xPosition, summonPosition1.position.y, 0), 0, false, null);
            BulletObject soldier = m_SoldierPool.Pop(new Vector3(xPosition, summonPosition1.position.y - 1.5f));
            m_Soldiers.Add(soldier);
            soldier.rigidbody2D.velocity = Vector2.up * 0.5f;

        }
       

        m_SoldierAppearingTimer = 3.7f;

        //m_SoldierCount++;
    }


    private void TeleportTo(Vector3 position)
    {
        VFXController.Instance.Trigger(m_VortexGroundEffectHash, position, 0, false, null);
        VFXController.Instance.Trigger(m_VortexGroundEffectHash, new Vector3(transform.position.x, transform.position.y  - 0.5f, 0), 0, false, null);

        m_RigidBody2D.velocity = Vector2.down * 0.7f;
        m_SpriteRenderer.sortingOrder = 0;
        m_TeleportToPosition = position;

        m_TeleportTimer = 4f;

    }

    private void TeleportToRandom()
    {
        float xPosition = Random.Range(summonPosition1.position.x, summonPosition2.position.x);

        TeleportTo(new Vector3(xPosition, summonPosition1.position.y, 0));
    }


    private bool SummonActionEndCheck()
    {
        for (int i = 0; i < m_Soldiers.Count; i++)
        {
            if (!m_Soldiers[i].inPool) return false;
        }

        return true;
    }


    public void GotHit(Damager damager, Damageable damageable)
    {
        m_Flicker.StartColorFickering(damageable.invulnerabilityDuration, 0.1f);
    }

}
