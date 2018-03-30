using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamekit2D;

public class Summoner : MonoBehaviour {

    public Transform targetToTrack;

    public GameObject rangeEnemyToSpawn;
    public GameObject meleeEnemyToSpawn;

    public GameObject fireBall;
    public GameObject concentratingAttack;
    public GameObject dashEffect;
    public float dashSpeed = 20f;

    public List<Transform> spawnRangeEnemyPositions;
    public List<Transform> spawnMeleeEnemyPositions;

    public Transform fireBallSpawnPosition1;
    public Transform fireBallSpawnPosition2;

    public Transform[] teleportPositions;

    private Animator m_Animator;

    private Rigidbody2D m_RigidBody2D;

    private BulletPool m_FireBallPool;
    private BulletPool m_ConcentratingAttackPool;

    //store spawned enemies and their spawned positions (in order to not spawn another enemy on top of an already spawned one)
    private List<KeyValuePair<Transform,GameObject>> m_SpawnedRangeEnemies = new List<KeyValuePair<Transform, GameObject>>();
    private List<KeyValuePair<Transform, GameObject>> m_SpawnedMeleeEnemies = new List<KeyValuePair<Transform, GameObject>>();


    private int m_HashFireBallAttackPara = Animator.StringToHash("FireBallAttack");
    private int m_HashExplodingAttackPara = Animator.StringToHash("ExplodingAttack");
    private int m_HashHurtPara = Animator.StringToHash("Hurt");
    private int m_HashTransformPara = Animator.StringToHash("Transform");
    private int m_HashEndAnimationPara = Animator.StringToHash("End"); 

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();

        m_RigidBody2D = GetComponentInParent<Rigidbody2D>();

        m_FireBallPool = BulletPool.GetObjectPool(fireBall, 10);
        m_ConcentratingAttackPool = BulletPool.GetObjectPool(concentratingAttack, 5);

    }

  
	// Update is called once per frame
	void Update () {
		
	}


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

   

    public void SpawnFireballs(int n)
    {
        StartCoroutine(InternalSpawnFireballs(n));


    }

    private IEnumerator InternalSpawnFireballs(int n)
    {
        m_Animator.SetTrigger(m_HashFireBallAttackPara);

        for (int i = 0; i < n; i++)
        {

            //position to spawn
            Vector3 spawnPosition = new Vector3(Random.Range(fireBallSpawnPosition1.position.x, fireBallSpawnPosition2.position.x), fireBallSpawnPosition1.position.y, 0);

            BulletObject fireBall = m_FireBallPool.Pop(spawnPosition);

            //direction from player to the fireball
            Vector3 direction = (targetToTrack.position - fireBall.transform.position).normalized;

            fireBall.rigidbody2D.velocity = direction * 5f;

            //rotate to player
            float rotationZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            fireBall.transform.rotation = Quaternion.Euler(0, 0, rotationZ);

            yield return new WaitForSeconds(2f);

        }

        m_Animator.SetTrigger(m_HashEndAnimationPara);
    }


    public void SpawnConcentratingAttack(int n)
    {

        StartCoroutine(InternalSpawnConcentraingAttack(n));
    }

    private IEnumerator InternalSpawnConcentraingAttack(int n)
    {
        m_Animator.SetTrigger(m_HashExplodingAttackPara);

        for (int i = 0; i < n; i++)
        {

            Vector3 spawnPosition = new Vector3(targetToTrack.transform.position.x,targetToTrack.transform.position.y+0.5f,0);

            BulletObject concentratingAttack = m_ConcentratingAttackPool.Pop(spawnPosition);

            Damager damager = concentratingAttack.bullet.GetComponent<Damager>();

            damager.DisableDamage();
            
            yield return new WaitForSeconds(0.8f);

            damager.EnableDamage();

            yield return new WaitForSeconds(0.5f);


        }

        m_Animator.SetTrigger(m_HashEndAnimationPara);
    }


    public void ChangeForm()
    {
        m_Animator.SetTrigger(m_HashTransformPara);
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

        //rotate to player
        float rotationZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rotationZ-90);

        m_RigidBody2D.velocity = direction * dashSpeed;

        yield return new WaitForSeconds(1f);

        dashEffect.SetActive(false);

        m_RigidBody2D.velocity = Vector3.zero;

        transform.rotation = Quaternion.identity;

    }




}
