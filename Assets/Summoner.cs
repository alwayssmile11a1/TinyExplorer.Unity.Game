using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Summoner : MonoBehaviour {

    public Transform targetToTrack;

    public GameObject rangeEnemyToSpawn;
    public GameObject meleeEnemyToSpawn;
    public GameObject fireBall;
    

    public List<Transform> spawnRangeEnemyPositions;
    public List<Transform> spawnMeleeEnemyPositions;

    public Transform fireBallSpawnPosition1;
    public Transform fireBallSpawnPosition2;

    public Transform[] teleportPositions;

    private Animator m_Animator;
    



    //store spawned enemies and their spawned positions (in order to not spawn another enemy on top of an already spawned one)
    private List<KeyValuePair<Transform,GameObject>> m_SpawnedRangeEnemies = new List<KeyValuePair<Transform, GameObject>>();
    private List<KeyValuePair<Transform, GameObject>> m_SpawnedMeleeEnemies = new List<KeyValuePair<Transform, GameObject>>();


    private void Awake()
    {
        m_Animator = GetComponent<Animator>();

        //use as a way to determine where to spawn fireBall 
        EdgeCollider2D m_EdgeCollider2D = GetComponent<EdgeCollider2D>();

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

        m_Animator.SetTrigger("StartSummoning");

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
        for (int i = 0; i < n; i++)
        {

            //position to spawn
            Vector3 spawnPosition = new Vector3(Random.Range(fireBallSpawnPosition1.position.x, fireBallSpawnPosition2.position.x), fireBallSpawnPosition1.position.y, 0);
            GameObject cloneFireBall = Instantiate(fireBall, spawnPosition, Quaternion.identity, transform);

            //direction from player to the fireball
            Vector3 direction = (targetToTrack.position - cloneFireBall.transform.position).normalized;

            Rigidbody2D rb2d = cloneFireBall.GetComponent<Rigidbody2D>();

            rb2d.velocity = direction * 5f;

            float rotationZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            cloneFireBall.transform.rotation = Quaternion.Euler(0, 0, rotationZ);

            Destroy(cloneFireBall, 20f);

            yield return new WaitForSeconds(2f);

        }
        

    }


    public void SpawnConcentratingAttack()
    {

    }


    public void Teleport()
    {
        
    }


    public void SelfCopy()
    {

    }


}
