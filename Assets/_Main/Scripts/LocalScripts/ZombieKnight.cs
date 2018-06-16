using BTAI;
using Gamekit2D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieKnight : MonoBehaviour{

    public Transform targetToTrack;

    [Header("Damager")]
    public Damager leftHeadDamager;
    public Damager rightHeadDamager;

    [Header("Slash")]
    public GameObject slash;

    //[Header("FireBalls")]
    //public Transform fireBallSpawnPosition1;
    //public Transform fireBallSpawnPosition2;
    //public GameObject fireBall;


    [Header("Portal")]
    public GameObject portal;
    public Transform portalPosition1;
    public Transform portalPosition2;


    [Header("DarkVoidPositions")]
    public GameObject darkVoid;
    public string darkVoidEffectName = "DarkVoid";
    public Transform darkVoidPosition1;
    public Transform darkVoidPosition2;

    [Header("Chaser")]
    public GameObject chaser;


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
    private Transform m_ParentTransform;
    private Vector3 m_OriginalGraphicsLocalScale;

    //Pools
    private BulletPool m_SlashPool;
    //private BulletPool m_FireBallPool;
    private BulletPool m_ChaserPool;
    private BulletPool m_PortalPool;
    private BulletPool m_DarkVoidPool;

    //private int m_FireBallCount = 10;

    //Portal
    private List<BulletObject> m_PortalObjects;
    private float m_PortalLiveTimer;
    private float m_PortalDelayTimer;
    private int m_PortalImpactHash;

    //DarkVoid
    private int m_DarkVoidHash;
    private Vector3 m_CurrentDarkVoidPosition;

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
        m_Damageable = GetComponentInParent<Damageable>();
        m_OriginalGraphicsLocalScale = transform.localScale;
        m_ParentTransform = transform.parent;
        m_SpriteRenderers = transform.GetComponentsInChildren<SpriteRenderer>();
        m_OriginalSpriteColors = new Color[m_SpriteRenderers.Length];
        for (int i = 0; i < m_SpriteRenderers.Length; i++)
        {
            m_OriginalSpriteColors[i] = m_SpriteRenderers[i].color;
        }

        //Pools
        m_SlashPool = BulletPool.GetObjectPool(slash, 3);
        //m_FireBallPool = BulletPool.GetObjectPool(fireBall, 10);
        m_ChaserPool = BulletPool.GetObjectPool(chaser, 5);
        m_PortalPool = BulletPool.GetObjectPool(portal, 3);
        m_DarkVoidPool = BulletPool.GetObjectPool(darkVoid, 1);

        //Effect
        m_DarkVoidHash = VFXController.StringToHash(darkVoidEffectName);
        m_PortalImpactHash = VFXController.StringToHash("ImpactEffect");

        m_PortalObjects = new List<BulletObject>();

        //Behaviour tree
        m_Ai.OpenBranch(
            BT.RandomSequence(new int[] { 1, 2, 3, 2, 3 }, 1).OpenBranch(
                //Walk
                BT.If(() => { return !MoveOutOfBoundCheck(); }).OpenBranch(
                    BT.Sequence().OpenBranch(
                        BT.Call(() => m_Animator.SetBool(m_HashWalkPara, true)),
                        BT.Call(() => SetHorizontalSpeed(1f)),
                        BT.Wait(2f),
                        BT.Call(() => SetHorizontalSpeed(0)),
                        BT.Call(() => m_Animator.SetBool(m_HashWalkPara, false)),
                        BT.Wait(0.5f)
                    )
                ),
                BT.If(() => { return !MoveOutOfBoundCheck(); }).OpenBranch(
                //Run
                BT.Sequence().OpenBranch(
                        BT.Call(() => m_Animator.SetTrigger(m_HashIdle2Para)),
                        BT.Wait(0.5f),
                        BT.WaitForAnimatorState(m_Animator, "idle_1"),
                        BT.Call(OrientToTarget),
                        BT.Call(() => m_Animator.SetBool(m_HashRunPara, true)),
                        BT.Call(() => SetHorizontalSpeed(5f)),
                        BT.Wait(2f),
                        BT.Call(() => SetHorizontalSpeed(0)),
                        BT.Call(() => m_Animator.SetBool(m_HashRunPara, false)),
                        BT.Wait(0.5f)
                    )
                ),

                //Slash
                BT.Sequence().OpenBranch(
                    BT.Call(() => m_Animator.SetTrigger(m_HashSkill1Para)),
                    BT.Wait(0.5f),
                    BT.WaitForAnimatorState(m_Animator, "idle_1"),
                    BT.Wait(0.5f)
                ),
                ////DarkVoid
                //BT.Sequence().OpenBranch(
                //    BT.Call(() => m_Animator.SetTrigger(m_HashSkill2Para)),
                //    BT.Wait(0.5f),
                //    BT.Call(SpawnRandomDarkVoid),
                //    BT.Wait(1.5f),
                //    BT.Repeat(5).OpenBranch(
                //        BT.Call(SpawnChaserAttack),
                //        BT.Wait(0.5f)
                //    )
                //),
                //DarkVoid Attacking
                BT.Sequence().OpenBranch(
                    BT.Call(() => m_Animator.SetTrigger(m_HashSkill2Para)),
                    BT.Wait(0.5f),
                    BT.Call(StartDarkVoidAttacking),
                    BT.Wait(3f)
                ),
                //Portal
                BT.If(() => { return m_Damageable.CurrentHealth <m_Damageable.startingHealth/1.5f && m_PortalObjects.Count <= 0; }).OpenBranch(
                    BT.Sequence().OpenBranch(
                        BT.Call(() => m_Animator.SetTrigger(m_HashSkill2Para)),
                        BT.Wait(0.5f),
                        BT.Call(SpawnPortal),
                        BT.WaitForAnimatorState(m_Animator, "idle_1")
                    )
                )
                ////Fireball
                //BT.Sequence().OpenBranch(
                //    BT.Call(() => m_Animator.SetTrigger(m_HashIdle2Para)),
                //    BT.Wait(0.5f),
                //    BT.WaitForAnimatorState(m_Animator, "idle_1"),
                //    BT.Wait(1f)
                //)

            ),

            BT.Call(OrientToTarget)

            //BT.Wait(0.5f)

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

        if (m_PortalLiveTimer > 0)
        {
            m_PortalLiveTimer -= Time.deltaTime;

            if(m_PortalLiveTimer<=0)
            {
                ClearPortals();
            }

        }


        if(m_PortalDelayTimer>0)
        {
            m_PortalDelayTimer -= Time.deltaTime;
            if(m_PortalDelayTimer<=0)
            {
                EnablePortal();
            }
        }

        if (MoveOutOfBoundCheck())
        {
            m_RigidBody2D.velocity = Vector2.zero;
        }


    }

    private bool MoveOutOfBoundCheck()
    {
        if (transform.position.x < portalPosition1.position.x && transform.localScale.x < 0 || transform.position.x > portalPosition2.position.x && transform.localScale.x >= 0)
        {
            return true;
        }

        return false;
    }

    private void OrientToTarget()
    {
        if (targetToTrack == null) return;

        if (targetToTrack.position.x < m_ParentTransform.position.x)
        {
            transform.localScale = m_OriginalGraphicsLocalScale;
            leftHeadDamager.EnableDamage();
            rightHeadDamager.DisableDamage();
        }
        else
        {
            Vector3 reverseScale = m_OriginalGraphicsLocalScale;
            reverseScale.x *= -1;
            transform.localScale = reverseScale;
            leftHeadDamager.DisableDamage();
            rightHeadDamager.EnableDamage();

        }
    }


    private void SetHorizontalSpeed(float speed)
    {
        m_RigidBody2D.velocity = speed * (transform.localScale.x < 0 ? Vector2.left : Vector2.right);

    }


    public void StartSlashAttacking()
    {
        for (int i = 0; i < 2; i++)
        {
            if (transform.localScale.x < 0)
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

    public void StartDarkVoidAttacking()
    {
        BulletObject darkVoidObject = m_DarkVoidPool.Pop(new Vector3(Random.Range(darkVoidPosition1.position.x, darkVoidPosition2.position.x), darkVoidPosition1.position.y, 0f));

        darkVoidObject.rigidbody2D.velocity = Vector2.down * 2f;

    }

    public void SpawnRandomDarkVoid()
    {
        m_CurrentDarkVoidPosition = new Vector3(Random.Range(darkVoidPosition1.position.x, darkVoidPosition2.position.x), darkVoidPosition1.position.y, 0f);
        VFXController.Instance.Trigger(m_DarkVoidHash, m_CurrentDarkVoidPosition, 0, false, null);
    }

    public void SpawnPortal()
    {
        m_PortalObjects.Clear();

        //Spawn Portal
        Vector3 randomPosition1 = new Vector3(portalPosition1.position.x + Random.Range(-0f,2f), portalPosition1.position.y, 0f);
        BulletObject portalObject1 = m_PortalPool.Pop(randomPosition1);
        m_PortalObjects.Add(portalObject1);

        Vector3 randomPosition2 = new Vector3(portalPosition2.position.x + Random.Range(-0f, -2f), portalPosition1.position.y, 0f);
        BulletObject portalObject2 = m_PortalPool.Pop(randomPosition2);
        m_PortalObjects.Add(portalObject2);


        //Link portals
        Portal portal1 = m_PortalObjects[0].transform.GetComponent<Portal>();
        Portal portal2 = m_PortalObjects[1].transform.GetComponent<Portal>();

        portal1.RemoveListener(Portalling);
        portal1.linkedPortal = portal2;
        portal1.AddListener(Portalling);
        portal1.CanPortal(false);

        portal2.RemoveListener(Portalling);
        portal2.linkedPortal = portal1;
        portal2.AddListener(Portalling);
        portal2.CanPortal(false);

        m_PortalLiveTimer = 12f;

        m_PortalDelayTimer = 1f;

    }

    private void Portalling(Portal portal, Portal linkedPortal, Collider2D collision)
    {
        //Slash 
        if (collision.transform.GetComponent<ZombieKnightSlash>() != null)
        {

            Transform slashTransform = collision.transform;
            Rigidbody2D slashRB2D = slashTransform.GetComponent<Rigidbody2D>();

            VFXController.Instance.Trigger(m_PortalImpactHash, slashTransform.position, 0.1f, false, null);
            VFXController.Instance.Trigger(m_PortalImpactHash, portal.transform.position, 0, false, null);

            slashTransform.position = new Vector2(linkedPortal.transform.position.x, linkedPortal.transform.position.y - 0.3f);

            if(slashTransform.position.x > targetToTrack.position.x)
            {
                slashTransform.GetComponent<SpriteRenderer>().flipX = true;
                slashRB2D.velocity = Vector2.left * 6f;
            }
            else
            {
                slashTransform.GetComponent<SpriteRenderer>().flipX = false;
                slashRB2D.velocity = Vector2.right * 6f;
            }

        }
        else
        {
            //Dark Matter
            if(collision.transform.GetComponent<Bullet>() != null)
            {
                Transform darkMatterTransform = collision.transform;
                Rigidbody2D darkMatterRB2D = darkMatterTransform.GetComponent<Rigidbody2D>();

                VFXController.Instance.Trigger(m_PortalImpactHash, darkMatterTransform.position, 0.1f, false, null);
                VFXController.Instance.Trigger(m_PortalImpactHash, portal.transform.position, 0, false, null);

                darkMatterTransform.position = new Vector2(linkedPortal.transform.position.x, linkedPortal.transform.position.y - 0.3f);

                if (darkMatterTransform.position.x > targetToTrack.position.x)
                {
                    darkMatterRB2D.velocity = Vector2.left * 6f;
                }
                else
                {
                    darkMatterRB2D.velocity = Vector2.right * 6f;
                }
            }
        }
        //else //ZombieKnight
        //{
        //    if (collision.transform.GetComponentInChildren<ZombieKnight>()!=null)
        //    {
        //        Debug.Log(m_RigidBody2D.position);

        //        m_ParentTransform.position = new Vector2(linkedPortal.transform.position.x, linkedPortal.transform.position.y - 1.2f);

        //        OrientToTarget();
        //        m_RigidBody2D.velocity = transform.localScale.x < 0 ? new Vector2(-Mathf.Abs(m_RigidBody2D.velocity.x), m_RigidBody2D.velocity.y)
        //                                                                      : new Vector2(Mathf.Abs(m_RigidBody2D.velocity.x), m_RigidBody2D.velocity.y);
        //    }

        //}

    }

    private void EnablePortal()
    {
        for (int i = 0; i < m_PortalObjects.Count; i++)
        {
            m_PortalObjects[i].transform.GetComponent<Portal>().CanPortal(true);
        }
    }

    private void ClearPortals()
    {
        for (int i = 0; i < m_PortalObjects.Count; i++)
        {
            m_PortalPool.Push(m_PortalObjects[i]);
        }
        m_PortalObjects.Clear();
    }

    public void SpawnChaserAttack()
    {
        BulletObject chaserObject = m_ChaserPool.Pop(m_CurrentDarkVoidPosition);
        chaserObject.transform.GetComponent<ChaseTarget>().StartChasing();
        chaserObject.transform.RotateTo(targetToTrack.position);
    }


    //public void SpawnFireballs()
    //{
    //    StartCoroutine(InternalSpawnFireballs());

    //}

    //private IEnumerator InternalSpawnFireballs()
    //{
    //    CameraShaker.Shake(0.03f, 2f * m_FireBallCount, false);

    //    for (int i = 0; i < m_FireBallCount; i++)
    //    {
    //        //position to spawn
    //        Vector3 spawnPosition = new Vector3(Random.Range(fireBallSpawnPosition1.position.x, fireBallSpawnPosition2.position.x), fireBallSpawnPosition1.position.y, 0);

    //        BulletObject fireBall = m_FireBallPool.Pop(spawnPosition);

    //        //direction from player to the fireball
    //        Vector3 direction = (targetToTrack.position - fireBall.transform.position).normalized;

    //        fireBall.rigidbody2D.velocity = direction * 10f;

    //        //rotate to player
    //        float rotationZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    //        fireBall.transform.rotation = Quaternion.Euler(0, 0, rotationZ);

    //        yield return new WaitForSeconds(1f);


    //    }

    //}


    public void GotHit(Damager damager, Damageable damageable)
    {
        StartFlickering();
    }

    public void Die(Damager damager, Damageable damageable)
    {
        m_RigidBody2D.velocity = Vector2.zero;
        GetComponentInParent<Damager>().DisableDamage();
        m_Animator.SetTrigger(m_HashDeathPara);

        StartCoroutine(DieEffectCoroutine());


    }

    private IEnumerator DieEffectCoroutine()
    {
        targetToTrack.GetComponent<CharacterInput>().SetInputActive(false);

        for (int i = 0; i < 50; i++)
        {

            //Death effect
            VFXController.Instance.Trigger("CFX_ExplosionPurple", transform.position + (Vector3)(Random.insideUnitCircle), 0.1f, false, null);


            yield return new WaitForSeconds(Mathf.Clamp(0.35f / (i + 1), 0.1f, 1f));

            ////Death effect
            //VFXController.Instance.Trigger(m_DeathEffectHash, transform.position + (Vector3)(Random.insideUnitCircle * 0.3f), 0.1f, false, null);

            //yield return new WaitForSeconds(0.1f);
        }


        VFXController.Instance.Trigger("SplashShieldHitRed", transform.position, 0.1f, false, null);

        transform.parent.gameObject.SetActive(false);

        targetToTrack.GetComponent<CharacterInput>().SetInputActive(true);

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
