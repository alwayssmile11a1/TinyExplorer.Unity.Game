using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Summoner : MonoBehaviour {

    public GameObject enemyToSpawn;

    public Transform[] spawnEnemyPositions;
    public Transform[] teleportPositions;

    private Animator m_Animator;

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
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

        if (enemyToSpawn != null || spawnEnemyPositions.Length == 0)
        {

            int randomIndex = Random.Range(0, spawnEnemyPositions.Length - 1);

            GameObject cloneEnemy = Instantiate(enemyToSpawn, spawnEnemyPositions[randomIndex].position, Quaternion.identity, transform);

            Destroy(cloneEnemy, 5f);



        }

    }


    public void SpawnFireballs()
    {

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
