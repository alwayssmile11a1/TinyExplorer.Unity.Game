using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Gamekit2D;
using BTAI;

public class ZombieKnight : MonoBehaviour, IBTDebugable{

    public Transform targetToTrack;

    [Header("Slash")]
    public GameObject slash;

    [Header("FireBalls")]
    public Transform fireBallSpawnPosition1;
    public Transform fireBallSpawnPosition2;
    public GameObject fireBall;


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


    //Pools
    private BulletPool m_SlashPool;
    private BulletPool m_FireBallPool;

    private int m_FireBallCount = 10;

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
        //Get references
        m_Animator = GetComponent<Animator>();
        m_RigidBody2D = GetComponentInParent<Rigidbody2D>();
        m_Damageable = GetComponent<Damageable>();

        m_GraphicsTransform = transform;
        m_OriginalGraphicsLocalScale = m_GraphicsTransform.localScale;

        m_SpriteRenderers = m_GraphicsTransform.GetComponentsInChildren<SpriteRenderer>();
        m_OriginalSpriteColors = new Color[m_SpriteRenderers.Length];
        for (int i = 0; i < m_SpriteRenderers.Length; i++)
        {
            m_OriginalSpriteColors[i] = m_SpriteRenderers[i].color;
        }

        //Pools
        m_SlashPool = BulletPool.GetObjectPool(slash, 3);
        m_FireBallPool = BulletPool.GetObjectPool(fireBall, 10);


        //Behaviour tree
        m_Ai.OpenBranch(

            BT.RandomSequence(new int[] { 2, 2, 2, 2, 2, 2 }, 2).OpenBranch(
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
                    BT.Call(() => m_Animator.SetTrigger(m_HashIdle2Para)),
                    BT.Wait(0.5f),
                    BT.WaitForAnimatorState(m_Animator, "idle_1"),
                    BT.Call(OrientToTarget),
                    BT.Call(() => m_Animator.SetBool(m_HashRunPara, true)),
                    BT.Call(() => SetHorizontalSpeed(5f)),
                    BT.Wait(2.5f),
                    BT.Call(() => SetHorizontalSpeed(0)),
                    BT.Call(() => m_Animator.SetBool(m_HashRunPara, false)),
                    BT.Wait(0.5f)
                ),
                //Slash
                BT.Sequence().OpenBranch(
                    BT.Call(() => m_Animator.SetTrigger(m_HashSkill1Para)),
                    BT.Wait(0.5f),
                    BT.WaitForAnimatorState(m_Animator, "idle_1"),
                    BT.Wait(1f)
                )
                ////Fireball
                //BT.Sequence().OpenBranch(
                //    BT.Call(() => m_Animator.SetTrigger(m_HashIdle2Para)),
                //    BT.Wait(0.5f),
                //    BT.WaitForAnimatorState(m_Animator, "idle_1"),
                //    BT.Wait(1f)
                //)

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


    public void StartSlashAttacking()
    {
        for (int i = 0; i < 2; i++)
        {
            if (m_GraphicsTransform.localScale.x < 0)
            {
                Vector3 position = transform.position + new Vector3(-1f - 0.6f * i, 0.9f, 0);
                BulletObject slashObject = m_SlashPool.Pop(position);
                slashObject.transform.GetComponent<SpriteRenderer>().flipX = true;
                slashObject.rigidbody2D.velocity = Vector2.left * 6f;
            }
            else
            {
                Vector3 position = transform.position + new Vector3(1f + 0.6f*i, 0.9f, 0);
                BulletObject slashObject = m_SlashPool.Pop(position);
                slashObject.transform.GetComponent<SpriteRenderer>().flipX = false;
                slashObject.rigidbody2D.velocity = Vector2.right * 6f;
            }
        }

    }

    public void SpawnFireballs()
    {
        StartCoroutine(InternalSpawnFireballs());

    }

    private IEnumerator InternalSpawnFireballs()
    {
        CameraShaker.Shake(0.03f, 2f * m_FireBallCount, false);

        for (int i = 0; i < m_FireBallCount; i++)
        {
            //position to spawn
            Vector3 spawnPosition = new Vector3(Random.Range(fireBallSpawnPosition1.position.x, fireBallSpawnPosition2.position.x), fireBallSpawnPosition1.position.y, 0);

            BulletObject fireBall = m_FireBallPool.Pop(spawnPosition);

            //direction from player to the fireball
            Vector3 direction = (targetToTrack.position - fireBall.transform.position).normalized;

            fireBall.rigidbody2D.velocity = direction * 10f;

            //rotate to player
            float rotationZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            fireBall.transform.rotation = Quaternion.Euler(0, 0, rotationZ);

            yield return new WaitForSeconds(1f);


        }

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
