using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamekit2D;
using BTAI;

public class ShardKnight : MonoBehaviour, IBTDebugable {

    public Transform targetToTrack;

    [Header("Chaser Attacks")]
    public GameObject[] chaserAttacks;

    [Header("Laser")]
    public Transform laserStartingPosition;
    public Transform laserFollower;
    public Gradient laserFollowAttackGradient;
    public Gradient circleLaserAttackGradient;

    public GameObject laserAttackBullet;

    [Header("Fireballs")]
    public GameObject fireBall;
    public Transform fireballRandomPosition1;
    public Transform fireballRandomPosition2;

    [Header("Exploding Attacks")]
    public GameObject concentratingAttack;

    [Header("Dash")]
    public GameObject dashEffect;
    public float dashSpeed = 30f;

    [Header("Move Positions")]
    public List<Transform> movePositions;

    [Header("Others")]
    public float timeBetweenFlickering = 0.1f;



    private Animator m_Animator;
    private Rigidbody2D m_RigidBody2D;
    private SpriteRenderer m_SpriteRenderer;
    private LineRenderer m_LineRenderer;
    private TrailRenderer m_trailRenderer;

    private Vector3[] chaserAttacksPositions;
    private BulletPool m_FireBallPool;
    private BulletPool m_ConcentratingAttackPool;
    private BulletPool m_LaserAttackPool;


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

    private int m_FireBalls = 5;
    private int m_ConcentratingAttacks = 5;
    private bool m_FormChanged = false;
    private Vector3 m_FuturePosition;
    private int m_CurrentMoveIndex;
    private FollowTarget m_LaserFollowComponent;
   

    //Laser
    private bool m_LaserEnabled = false;
    private float m_LaserAttackShotAngle = 0;
    private float m_LaserSweptAngle;
    private float laserAttackType; //1 is laserFollowAttack, 2 and 3 is circleLaseAttack

    private Root m_Ai = BT.Root();

    private void Awake()
    {
        m_Animator = GetComponentInChildren<Animator>();
        m_RigidBody2D = GetComponent<Rigidbody2D>();
        m_Damageable = GetComponent<Damageable>();
        m_SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        m_LineRenderer = GetComponentInChildren<LineRenderer>();
        m_Flicker = m_Animator.gameObject.AddComponent<Flicker>();
        m_LaserFollowComponent = laserFollower.GetComponent<FollowTarget>();
        m_trailRenderer = GetComponentInChildren<TrailRenderer>();

        //Pool
        m_FireBallPool = BulletPool.GetObjectPool(fireBall, 5);
        m_ConcentratingAttackPool = BulletPool.GetObjectPool(concentratingAttack, 5);
        m_LaserAttackPool = BulletPool.GetObjectPool(laserAttackBullet, 20);

        //Setup chaserAttacks
        chaserAttacksPositions = new Vector3[chaserAttacks.Length];
        for (int i = 0; i < chaserAttacks.Length; i++)
        {
            chaserAttacksPositions[i] = chaserAttacks[i].transform.position;
        }

        //Behaviour tree
        m_Ai.OpenBranch(
            BT.If(() => { return m_Damageable.CurrentHealth >= m_Damageable.startingHealth / 2; }).OpenBranch(

                 BT.RandomSequence(new int[] { 2, 1, 3 }).OpenBranch(
                    //Laser follow attack
                    BT.Sequence().OpenBranch(
                        BT.Call(() => laserAttackType = 1),
                        BT.Call(() => m_LineRenderer.colorGradient = laserFollowAttackGradient),
                        BT.Call(StartLaserAttack),
                        BT.Wait(1.5f),
                        BT.Call(() => m_LaserFollowComponent.speed = 1f),
                        BT.Call(() => m_LineRenderer.enabled = false),
                        BT.Repeat(15).OpenBranch(
                            BT.Call(LaserFollowAttack),
                            BT.Wait(0.1f)
                        ),
                        BT.Call(() => m_LaserFollowComponent.speed = 2f),
                        BT.Call(EndLaserAttack),
                        BT.Wait(1f)
                    ),
                    //Cicle laser Attack
                    BT.Sequence().OpenBranch(
                        BT.Call(() => laserAttackType = Random.Range(2, 4)),
                        BT.Call(() => m_LineRenderer.colorGradient = circleLaserAttackGradient),
                        BT.Call(StartLaserAttack),
                        BT.WaitUntil(() => Mathf.Abs(m_LaserSweptAngle) >= 360),
                        BT.Call(() => m_LineRenderer.enabled = false),
                        BT.Repeat(19).OpenBranch(
                            BT.Call(CircleLaserAttack),
                            BT.Wait(0.1f)
                        ),
                        BT.Call(EndLaserAttack),
                        BT.Wait(1f)
                    ),
                    //Dash
                    BT.Sequence().OpenBranch(
                        BT.Call(DashToLower, 5f, false),
                        BT.WaitUntil(MoveCheck),
                        BT.Wait(0.5f),
                        BT.Call(StartDashing),
                        BT.Call(DashToLower, 10f, true),
                        BT.Call(RotateTowardsFuturePosition), 
                        BT.WaitUntil(MoveCheck),
                        BT.Call(EndDashing),
                        BT.Wait(0.5f),
                        BT.Call(FlipSpriteBasedOnSide),
                        BT.Call(DashToUpper, 10f, false),
                        BT.WaitUntil(MoveCheck),
                        BT.Wait(1f)
                    )

                )
            ),

            BT.If(() => { return m_Damageable.CurrentHealth <= m_Damageable.startingHealth / 2; }).OpenBranch(
                //Change form animation
                BT.If(() => { return !m_FormChanged; }).OpenBranch(
                    BT.Call(ChangeForm),
                    BT.WaitForAnimatorState(m_Animator, "ShardKnightAfterTransform"),
                    BT.Wait(1f)
                 ),

                BT.RandomSequence(new int[] { 2, 3, 1 }).OpenBranch(
                    //Dash
                    BT.Sequence().OpenBranch(
                        BT.Call(StartDashing),
                        BT.Call(DashToLower, 12f, true),
                        BT.Call(RotateTowardsFuturePosition),
                        BT.WaitUntil(MoveCheck),
                        BT.Wait(0.2f),
                        BT.Call(FlipSpriteBasedOnSide),
                        BT.Call(DashToLower, 12f, true),
                        BT.Call(RotateTowardsFuturePosition),
                        BT.WaitUntil(MoveCheck),
                        BT.Wait(0.2f),
                        BT.Call(FlipSpriteBasedOnSide),
                        BT.Call(DashToUpper, 12f, true),
                        BT.Call(RotateTowardsFuturePosition),
                        BT.WaitUntil(MoveCheck),
                        BT.Call(EndDashing),
                        BT.Call(FlipSpriteBasedOnSide),
                        BT.Wait(1f)

                    ),
                    //Exploding Attack
                    BT.Sequence().OpenBranch(
                        BT.Call(SpawnConcentratingAttack),
                        BT.WaitForAnimatorState(m_Animator, "ShardKnightAfterTransform"),
                        BT.Wait(2f)
                    ),
                    //Cicle laser Attack
                    BT.Sequence().OpenBranch(
                        BT.Call(() => laserAttackType = Random.Range(2, 4)),
                        BT.Call(() => m_LineRenderer.colorGradient = circleLaserAttackGradient),
                        BT.Call(StartLaserAttack),
                        BT.WaitUntil(() => Mathf.Abs(m_LaserSweptAngle) >= 360),
                        BT.Call(() => m_LineRenderer.enabled = false),
                        BT.Repeat(19).OpenBranch(
                            BT.Call(CircleLaserAttack),
                            BT.Wait(0.1f)
                        ),
                        BT.Call(EndLaserAttack),
                        BT.Wait(1f)
                    )


                 ),

                BT.Call(OrientToTarget)
              )

            
        );

    }

    public Root GetAIRoot()
    {
        return m_Ai;
    }
 

    // Update is called once per frame
    void Update () {


        m_Ai.Tick();

        if(m_LaserEnabled)
        {
            if (laserAttackType == 1)
            {
                TrackTargerByLaser();
            }
            else
            {
                DrawLaserFullCircle();
            }
        }
       

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


    private void SpawnConcentratingAttack()
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


    private void ChangeForm()
    {
        m_Animator.SetTrigger(m_HashTransformPara);
        m_FormChanged = true;
    }
    
    private bool MoveCheck()
    {
        if ((transform.position - m_FuturePosition).sqrMagnitude <= 0.5f)
        {
            m_RigidBody2D.velocity = Vector2.zero;
            return true;
        }

        return false;
    }

    private void MoveTo(Vector3 position, float speed)
    {
        Vector3 direction = (position - transform.position).normalized;

        m_RigidBody2D.velocity = direction * speed;

        //transform.position = Vector3.Slerp(transform.position, position, speed);

    }

    private void DashToLower(float speed, bool switchSide = true)
    {
        if (m_CurrentMoveIndex == 2 || m_CurrentMoveIndex == 0)
        {
            if (switchSide)
            {
                MoveTo(movePositions[1].position, speed);
                m_FuturePosition = movePositions[1].position;
                m_CurrentMoveIndex = 1;
            }
            else
            {
                MoveTo(movePositions[0].position, speed);
                m_FuturePosition = movePositions[0].position;
                m_CurrentMoveIndex = 0;
            }
        }
        else
        {
            if (switchSide)
            {
                MoveTo(movePositions[0].position, speed);
                m_FuturePosition = movePositions[0].position;
                m_CurrentMoveIndex = 0;
            }
            else
            {
                MoveTo(movePositions[1].position, speed);
                m_FuturePosition = movePositions[1].position;
                m_CurrentMoveIndex = 1;
            }

        }
    }

    private void DashToUpper(float speed, bool switchSide = true)
    {
        Vector3 futurePosition;

        if (m_CurrentMoveIndex == 2 || m_CurrentMoveIndex == 0)
        {
            if (switchSide)
            {
                futurePosition = movePositions[3].position;
                futurePosition.x = Random.Range(movePositions[3].position.x, movePositions[4].position.x);
                m_CurrentMoveIndex = 3;
            }
            else
            {
                futurePosition = movePositions[2].position;
                futurePosition.x = Random.Range(movePositions[2].position.x, movePositions[4].position.x);
                m_CurrentMoveIndex = 2;
            }
        }
        else
        {
            if (switchSide)
            {
                futurePosition = movePositions[2].position;
                futurePosition.x = Random.Range(movePositions[2].position.x, movePositions[4].position.x);
                m_CurrentMoveIndex = 2;
            }
            else
            {
                futurePosition = movePositions[3].position;
                futurePosition.x = Random.Range(movePositions[3].position.x, movePositions[4].position.x);
                m_CurrentMoveIndex = 3;
            }

        }

        MoveTo(futurePosition, speed);
        m_FuturePosition = futurePosition;
    }

    private void StartDashing()
    { 
        dashEffect.SetActive(true);
        m_trailRenderer.enabled = true;
    }

    private void EndDashing()
    {  
        dashEffect.SetActive(false);
        m_trailRenderer.enabled = false;
        transform.rotation = Quaternion.identity;
        m_RigidBody2D.velocity = Vector2.zero;
    }

    private void RotateTowardsFuturePosition()
    {
        Vector3 direction = (m_FuturePosition - transform.position).normalized;

        //rotate shardknight
        float rotationZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rotationZ - 90);
    }

    private void FlipSpriteBasedOnSide()
    {
        if (m_CurrentMoveIndex == 2 || m_CurrentMoveIndex == 0)
        {
            m_SpriteRenderer.flipX = false;
        }
        else
        {
            m_SpriteRenderer.flipX = true;
        }
    }




    private void StartLaserAttack()
    {
        laserFollower.gameObject.SetActive(true);
        m_LineRenderer.enabled = true;
        m_LaserEnabled = true;
        m_LaserAttackShotAngle = 0;
        m_LaserSweptAngle = 0;
        m_LineRenderer.SetPosition(0, laserStartingPosition.position);
    }

    private void EndLaserAttack()
    {
        laserFollower.gameObject.SetActive(false);
        m_LineRenderer.enabled = false;
        m_LaserEnabled = false;
    }
    
    private void TrackTargerByLaser()
    {    
        Vector3 direction = laserFollower.position - laserStartingPosition.position;

        //if the laser hit something
        RaycastHit2D m_RaycastHit2D = Physics2D.Raycast(laserStartingPosition.position, direction, 100, LayerMask.GetMask("Player","Platform"));

        if (m_RaycastHit2D)
        {
            m_LineRenderer.SetPosition(1, m_RaycastHit2D.point);
        }
        else
        {
            m_LineRenderer.SetPosition(1, laserStartingPosition.position + direction * 100);

        }

    }

    private void DrawLaserFullCircle()
    {
        Vector3 direction = Quaternion.Euler(0, 0, m_LaserSweptAngle) * Vector2.down;

        //if the laser hit something
        RaycastHit2D m_RaycastHit2D = Physics2D.Raycast(laserStartingPosition.position, direction, 100, LayerMask.GetMask("Player", "Platform"));

        if (m_RaycastHit2D)
        {
            m_LineRenderer.SetPosition(1, m_RaycastHit2D.point);
        }
        else
        {
            m_LineRenderer.SetPosition(1, laserStartingPosition.position + direction * 100);

        }

        if (laserAttackType == 2)
        {
            m_LaserSweptAngle += 200 * Time.deltaTime;
        }
        else
        {
            m_LaserSweptAngle -= 200 * Time.deltaTime;
        }

    }

    private void LaserFollowAttack()
    {
        Vector2 startingShootPosition1 = new Vector3(-0.45f, 0.45f,0) + transform.position;
        Vector2 startingShootPosition2 = new Vector3(0.45f, 0.45f,0) + transform.position;
        
        //get bullet object
        BulletObject laserAttackBulletObject = m_LaserAttackPool.Pop(Random.Range(0, 2) == 1 ? startingShootPosition1 : startingShootPosition2);
        
        Vector2 direction = (laserFollower.position - laserAttackBulletObject.transform.position).normalized;

        //rotate
        laserAttackBulletObject.transform.RotateToDirection(direction);

        laserAttackBulletObject.rigidbody2D.velocity = direction * 15f;
    }

    private void CircleLaserAttack()
    {
        Vector2 startingShootPosition = transform.position;

        //get bullet object
        BulletObject laserAttackBulletObject = m_LaserAttackPool.Pop(startingShootPosition);

        Vector2 direction = (Quaternion.Euler(0,0, m_LaserAttackShotAngle) * Vector2.down).normalized;

        //rotate
        laserAttackBulletObject.transform.RotateToDirection(direction);

        laserAttackBulletObject.rigidbody2D.velocity = direction * 10f;

        if (laserAttackType == 2)
        {
            m_LaserAttackShotAngle += 20;
        }
        else
        {
            m_LaserAttackShotAngle -= 20;
        }
    }



    public void GotHit(Damager damager, Damageable damageable)
    {
        m_Flicker.StartFlickering(damageable.invulnerabilityDuration, timeBetweenFlickering);


        ////Teleport
        //int random1 = Random.Range(0, teleportPositions.Count);
        //Transform randomTransform1 = teleportPositions[random1];
        //teleportPositions.RemoveAt(random1);

        //int random2 = Random.Range(0, teleportPositions.Count);
        //Transform randomTransform2 = teleportPositions[random2];
        //teleportPositions.RemoveAt(random2);


        ////position to spawn
        //Vector3 teleportPosition = new Vector3(Random.Range(randomTransform1.position.x, randomTransform2.position.x),
        //                                                Random.Range(randomTransform1.position.y, randomTransform2.position.y), 0);


        ////teleport
        //transform.position = teleportPosition;


        ////Add to teleport position again
        //teleportPositions.Add(randomTransform1);
        //teleportPositions.Add(randomTransform2);
    }

}
