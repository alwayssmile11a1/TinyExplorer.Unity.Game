using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamekit2D;
using BTAI;

public class ShardKnight : MonoBehaviour {

    public Transform targetToTrack;

    [Header("Chaser Attacks")]
    public GameObject[] chaserAttacks;    

    [Header("Fireballs")]
    public GameObject fireBall;
    public Transform fireballRandomPosition1;
    public Transform fireballRandomPosition2;

    [Header("Exploding Attacks")]
    public GameObject concentratingAttack;

    [Header("Dash")]
    public GameObject dashEffect;
    public float dashSpeed = 30f;

    [Header("Teleport")]
    public List<Transform> teleportPositions;

    [Header("Others")]
    public float timeBetweenFlickering = 0.1f;



    private Animator m_Animator;
    private Rigidbody2D m_RigidBody2D;
    private SpriteRenderer m_SpriteRenderer;

    private Vector3[] chaserAttacksPositions;

    private BulletPool m_FireBallPool;
    private BulletPool m_ConcentratingAttackPool;

    #region Deprecated
    private GameObject rangeEnemyToSpawn;
    private GameObject meleeEnemyToSpawn;
    private List<Transform> spawnRangeEnemyPositions;
    private List<Transform> spawnMeleeEnemyPositions;
    //store spawned enemies and their spawned positions (in order to not spawn another enemy on top of an already spawned one)
    private List<KeyValuePair<Transform,GameObject>> m_SpawnedRangeEnemies = new List<KeyValuePair<Transform, GameObject>>();
    private List<KeyValuePair<Transform, GameObject>> m_SpawnedMeleeEnemies = new List<KeyValuePair<Transform, GameObject>>();
    #endregion

    private int m_HashFireBallAttackPara = Animator.StringToHash("FireBallAttack");
    private int m_HashExplodingAttackPara = Animator.StringToHash("ExplodingAttack");
    private int m_HashHurtPara = Animator.StringToHash("Hurt");
    private int m_HashTransformPara = Animator.StringToHash("Transform");
    private int m_HashEndAnimationPara = Animator.StringToHash("End");
    private Flicker m_Flicker;
    private Damageable m_Damageable;

    private Root m_Ai = BT.Root();

    private int m_FireBalls = 5;
    private int m_ConcentratingAttacks = 5;
    private bool m_FormChanged = false;

    private void Awake()
    {
        m_Animator = GetComponentInChildren<Animator>();
        m_RigidBody2D = GetComponent<Rigidbody2D>();
        m_Damageable = GetComponent<Damageable>();
        m_SpriteRenderer = GetComponentInChildren<SpriteRenderer>();

        m_Flicker = m_Animator.gameObject.AddComponent<Flicker>();

        //Pool
        m_FireBallPool = BulletPool.GetObjectPool(fireBall, 10);
        m_ConcentratingAttackPool = BulletPool.GetObjectPool(concentratingAttack, 5);

        //Setup chaserAttacks
        chaserAttacksPositions = new Vector3[chaserAttacks.Length];
        for (int i = 0; i < chaserAttacks.Length; i++)
        {
            chaserAttacksPositions[i] = chaserAttacks[i].transform.position;
        }

        //Behaviour tree
        m_Ai.OpenBranch(
            BT.If(() => { return m_Damageable.CurrentHealth > m_Damageable.startingHealth / 2; }).OpenBranch(

                 BT.RandomSequence(new int[] { 1, 1 }).OpenBranch(
                    BT.Sequence().OpenBranch(
                            BT.Call(SpawnChaserAttacks),
                            BT.Wait(5f)
                        ),

                    BT.Sequence().OpenBranch(
                            BT.Call(SpawnFireballs),
                            BT.Wait(2f * m_FireBalls)
                        )
                    )
                ),

            BT.If(() => { return m_Damageable.CurrentHealth < m_Damageable.startingHealth / 2; }).OpenBranch(

                BT.If(() => { return !m_FormChanged; }).OpenBranch(
                    BT.Call(ChangeForm),
                    BT.WaitForAnimatorState(m_Animator, "ShardKnightAfterTransform"),
                    BT.Wait(2f)
                 ),

                BT.RandomSequence(new int[] { 3, 1, 3 }).OpenBranch(

                    BT.Repeat(5).OpenBranch(
                        BT.Call(Dash),
                        BT.Wait(2f)
                    ),

                    BT.Sequence().OpenBranch(
                        BT.Call(SpawnFireballs),
                        BT.Wait(1.5f * m_FireBalls)
                    ),

                    BT.Sequence().OpenBranch(
                        BT.Call(SpawnConcentratingAttack),
                        BT.Wait(1.5f * m_ConcentratingAttacks)
                    )
                 ),

                BT.Call(OrientToTarget)
              )

        );

    }

    // Update is called once per frame
    void Update () {


        m_Ai.Tick();

        OrientToTarget();


    }

    private void OrientToTarget()
    {
        if (targetToTrack == null) return;

        if(targetToTrack.position.x > transform.position.x)
        {
            m_SpriteRenderer.flipX = true;
        }
        else
        {
            m_SpriteRenderer.flipX = false;
        }


        
    }

    #region Deprecated
    public void SpawnEnemies()
    {
        StartCoroutine(InternalSpawnEnemies());
    }

    private IEnumerator InternalSpawnEnemies()
    {

       
        yield return new WaitForSeconds(2f);



        //random the number of enemies to spawn
        int numberOfEnemies = Random.Range(1, 3);


        for (int i = 0; i < numberOfEnemies; i++)
        {
            //random type of enemies to spawn (1 is range, 2 is melee)
            int type = Random.Range(1, 3);

            switch (type)
            {
                case 1:
                    {
                        if (!SpawnEnemy(rangeEnemyToSpawn, spawnRangeEnemyPositions, m_SpawnedRangeEnemies))
                        {
                            SpawnEnemy(meleeEnemyToSpawn, spawnMeleeEnemyPositions, m_SpawnedMeleeEnemies);
                        }

                        break;
                    }

                case 2:
                    {
                        if (!SpawnEnemy(meleeEnemyToSpawn, spawnMeleeEnemyPositions, m_SpawnedMeleeEnemies))
                        {
                            SpawnEnemy(rangeEnemyToSpawn, spawnRangeEnemyPositions, m_SpawnedRangeEnemies);
                        }
                        break;
                    }
            }
        }

    }

    private bool SpawnEnemy(GameObject enemyToSpawn,List<Transform> positions, List<KeyValuePair<Transform, GameObject>> spawnedEnemies)
    {

        if (enemyToSpawn == null) return false;

        //find and remove null value from m_SpawnedRangeEnemies
        for (int i = 0; i < spawnedEnemies.Count; i++)
        {
            if (spawnedEnemies[i].Value == null)
            {
                positions.Add(spawnedEnemies[i].Key);
                spawnedEnemies.RemoveAt(i);
            }
        }

        if (positions.Count == 0) return false;

        //get random position to spawn an enemy
        int randomIndex = Random.Range(0, positions.Count);
        Transform spawnPosition = positions[randomIndex];
        positions.RemoveAt(randomIndex);

        //spawn enemy
        GameObject cloneEnemy = Instantiate(enemyToSpawn, spawnPosition.position, Quaternion.identity, transform);

        //add to list of spawned enemies
        spawnedEnemies.Add(new KeyValuePair<Transform, GameObject>(spawnPosition, cloneEnemy));


        return true;

    }

    #endregion

    public void SpawnChaserAttacks()
    {
        StartCoroutine(InternalSpawnChaserAttacks());
    }

    private IEnumerator InternalSpawnChaserAttacks()
    {
        for (int i = 0; i < chaserAttacks.Length; i++)
        {
 
            chaserAttacks[i].SetActive(true);
            chaserAttacks[i].GetComponent<ChaseTarget>().StopChasing();

            //return to original position
            chaserAttacks[i].transform.position = chaserAttacksPositions[i];

            yield return new WaitForSeconds(0.75f);

        }

        //chaser start attacking
        for (int i = 0; i < chaserAttacks.Length; i++)
        {
            chaserAttacks[i].GetComponent<ChaseTarget>().StartChasing();
        }

    }

    public void OnChaserHit(Damager damager)
    {
        GameObject chaser = damager.gameObject;
        chaser.SetActive(false);
        chaser.GetComponent<ChaseTarget>().StopChasing();
    }


    public void SpawnFireballs()
    {
        StartCoroutine(InternalSpawnFireballs());


    }

    private IEnumerator InternalSpawnFireballs()
    {
        CameraShaker.Shake(0.03f, 2f* m_FireBalls, false);

        m_Animator.SetTrigger(m_HashFireBallAttackPara);

        for (int i = 0; i < m_FireBalls; i++)
        {
            //position to spawn
            Vector3 spawnPosition = new Vector3(Random.Range(fireballRandomPosition1.position.x, fireballRandomPosition2.position.x), fireballRandomPosition1.position.y, 0);

            BulletObject fireBall = m_FireBallPool.Pop(spawnPosition);

            //direction from player to the fireball
            Vector3 direction = (targetToTrack.position - fireBall.transform.position).normalized;

            fireBall.rigidbody2D.velocity = direction * 10f;

            //rotate to player
            float rotationZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            fireBall.transform.rotation = Quaternion.Euler(0, 0, rotationZ);

            yield return new WaitForSeconds(1f);

            
        }

        m_Animator.SetTrigger(m_HashEndAnimationPara);
    }


    public void SpawnConcentratingAttack()
    {
        StartCoroutine(InternalSpawnConcentraingAttacks());
    }

    private IEnumerator InternalSpawnConcentraingAttacks()
    {
        m_Animator.SetTrigger(m_HashExplodingAttackPara);

        for (int i = 0; i < m_ConcentratingAttacks; i++)
        {

            Vector3 spawnPosition = new Vector3(targetToTrack.transform.position.x,targetToTrack.transform.position.y+0.5f,0);

            BulletObject concentratingAttack = m_ConcentratingAttackPool.Pop(spawnPosition);

            Damager damager = concentratingAttack.bullet.GetComponent<Damager>();

            damager.DisableDamage();
            
            yield return new WaitForSeconds(0.8f);

            CameraShaker.Shake(0.05f, 0.1f);

            damager.EnableDamage();

            yield return new WaitForSeconds(0.5f);

            

        }

        m_Animator.SetTrigger(m_HashEndAnimationPara);
    }


    public void ChangeForm()
    {
        m_Animator.SetTrigger(m_HashTransformPara);
        m_FormChanged = true;
    }

    
    public void Dash()
    {

        StartCoroutine(InternalDash());

    }


    private IEnumerator InternalDash()
    {
        dashEffect.SetActive(true);

        Vector3 targetPosition = new Vector3(targetToTrack.transform.position.x, targetToTrack.transform.position.y + 0.5f, 0);

        Vector3 direction = (targetPosition - transform.position).normalized;

        //rotate shardknight
        float rotationZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rotationZ-90);

        m_RigidBody2D.velocity = direction * dashSpeed;

        yield return new WaitForSeconds(1f);

        dashEffect.SetActive(false);

        m_RigidBody2D.velocity = Vector3.zero;

        transform.rotation = Quaternion.identity;

    }


    public void GotHit(Damager damager, Damageable damageable)
    {
        m_Flicker.StartFlickering(damageable.invulnerabilityDuration, timeBetweenFlickering);


        //Teleport
        int random1 = Random.Range(0, teleportPositions.Count);
        Transform randomTransform1 = teleportPositions[random1];
        teleportPositions.RemoveAt(random1);

        int random2 = Random.Range(0, teleportPositions.Count);
        Transform randomTransform2 = teleportPositions[random2];
        teleportPositions.RemoveAt(random2);


        //position to spawn
        Vector3 teleportPosition = new Vector3(Random.Range(randomTransform1.position.x, randomTransform2.position.x),
                                                        Random.Range(randomTransform1.position.y, randomTransform2.position.y), 0);


        //teleport
        transform.position = teleportPosition;


        //Add to teleport position again
        teleportPositions.Add(randomTransform1);
        teleportPositions.Add(randomTransform2);
    }

}
