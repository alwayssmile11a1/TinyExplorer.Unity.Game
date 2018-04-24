using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Gamekit2D;
using BTAI;

public class ZombieKnight : MonoBehaviour, IBTDebugable{

    public Transform targetToTrack;

    [Header("Damagers")]
    public Damager shortDamager;

    [Header("Flicker")]
    public Color flickerColor = new Color(1f,100/255f, 100/255f, 1f);
    public float flickerDuration = 0.1f;
    public float timeBetweenFlickering = 0.1f;

    //Animation
    private int m_HashIdle1Para = Animator.StringToHash("Idle");
    private int m_HashIdle2Para = Animator.StringToHash("Idle2");
    private int m_HashWalkPara = Animator.StringToHash("Walk");
    private int m_HashRunPara = Animator.StringToHash("Run");
    private int m_HashDeathPara = Animator.StringToHash("Death");
    private int m_HashSkill1Para = Animator.StringToHash("Skill1");
    private int m_HashSkill2Para = Animator.StringToHash("Skill2");

    //References
    private Animator m_Animator;
    private Rigidbody2D m_RigidBody2D;
    private Damageable m_Damageable;
    private Transform m_GraphicsTransform;
    private Vector3 m_OriginalGraphicsLocalScale;


    //Custom flickering
    private SpriteRenderer[] m_SpriteRenderers;
    private Color[] m_OriginalSpriteColors;
    private int m_State;
    private float m_FlickerTimer;
    private float m_SinceLastChange;

    //Behavior Tree
    private Root m_Ai = BT.Root();


    private void Awake()
    {
        m_Animator = GetComponentInChildren<Animator>();
        m_RigidBody2D = GetComponent<Rigidbody2D>();
        m_Damageable = GetComponent<Damageable>();

        m_GraphicsTransform = m_Animator.GetComponent<Transform>();
        m_OriginalGraphicsLocalScale = m_GraphicsTransform.localScale;

        m_SpriteRenderers = m_GraphicsTransform.GetComponentsInChildren<SpriteRenderer>();
        m_OriginalSpriteColors = new Color[m_SpriteRenderers.Length];
        for (int i = 0; i < m_SpriteRenderers.Length; i++)
        {
            m_OriginalSpriteColors[i] = m_SpriteRenderers[i].color;
        }



        //Behaviour tree
        m_Ai.OpenBranch(

            BT.RandomSequence(new int[] { 2, 1, 2, 2, 2, 1 }, 2).OpenBranch(
                //Walk
                BT.Sequence().OpenBranch(
                    BT.Call(() => m_Animator.SetBool(m_HashWalkPara, true)),
                    BT.Call(() => SetHorizontalSpeed(1f)),
                    BT.Wait(2.5f),
                    BT.Call(() => SetHorizontalSpeed(0)),
                    BT.Call(() => m_Animator.SetBool(m_HashWalkPara, false)),
                    BT.Wait(0.5f)
                ),
                //Run
                BT.Sequence().OpenBranch(
                    BT.Call(() => m_Animator.SetBool(m_HashRunPara, true)),
                    BT.Call(() => SetHorizontalSpeed(3f)),
                    BT.Wait(2.5f),
                    BT.Call(() => SetHorizontalSpeed(0)),
                    BT.Call(() => m_Animator.SetBool(m_HashRunPara, false)),
                    BT.Wait(0.5f)
                ),
                //Skill1
                BT.Sequence().OpenBranch(
                    BT.Call(() => m_Animator.SetTrigger(m_HashSkill1Para)),
                    BT.Wait(0.5f),
                    BT.WaitForAnimatorState(m_Animator,"idle_1"),
                    BT.Wait(1f)
                ),
                //Skill2
                BT.Sequence().OpenBranch(
                    BT.Call(() => m_Animator.SetTrigger(m_HashSkill2Para)),
                    BT.Wait(0.5f),
                    BT.WaitForAnimatorState(m_Animator, "idle_1"),
                    BT.Wait(1f)
                )

            ),

            BT.Call(OrientToTarget),

            BT.Wait(1f)



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


        if(m_FlickerTimer>0)
        {
            Flicker();

            m_FlickerTimer -= Time.deltaTime;

            if(m_FlickerTimer<=0)
            {
                StopFlickering();
            }

        }

    }

    private void OrientToTarget()
    {
        if (targetToTrack == null) return;

        if (targetToTrack.position.x < transform.position.x)
        {
            m_GraphicsTransform.localScale = m_OriginalGraphicsLocalScale;
        }
        else
        {
            Vector3 reverseScale = m_OriginalGraphicsLocalScale;
            reverseScale.x *= -1;
            m_GraphicsTransform.localScale = reverseScale;
        }
    }


    private void SetHorizontalSpeed(float speed)
    {
        m_RigidBody2D.velocity = speed * (m_GraphicsTransform.localScale.x < 0 ? Vector2.left : Vector2.right);

    }


    public void GotHit(Damager damager, Damageable damageable)
    {
        StartFlickering();
    }

    public void StartFlickering()
    {
        m_State = 1;
        m_SinceLastChange = 0;
        m_FlickerTimer = flickerDuration;

        for (int i = 0; i < m_SpriteRenderers.Length; i++)
        {
            m_SpriteRenderers[i].color = flickerColor;
        }

    }

    private void Flicker()
    {   
        m_SinceLastChange += Time.deltaTime;
        if (m_SinceLastChange > timeBetweenFlickering)
        {
            m_SinceLastChange -= timeBetweenFlickering;
            m_State = 1 - m_State;

            for (int i = 0; i < m_SpriteRenderers.Length; i++)
            {
                m_SpriteRenderers[i].color = m_State == 1 ? flickerColor : m_OriginalSpriteColors[i];
            }
        }
    }

    public void StopFlickering()
    {
        for (int i = 0; i < m_SpriteRenderers.Length; i++)
        {
            m_SpriteRenderers[i].color = m_OriginalSpriteColors[i];
        }

    }


}
