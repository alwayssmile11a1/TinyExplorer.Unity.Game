using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBoss : MonoBehaviour {
    public GameObject boss;
    public GameObject AppearEffect;
    private BoxCollider2D boxCollider2D;
    private void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Player"))
        {
            if(AppearEffect != null && !AppearEffect.activeSelf)
                AppearEffect.SetActive(true);
            StartCoroutine(SpawnAlicia());
            Destroy(boxCollider2D);
        }
    }

    private IEnumerator SpawnAlicia()
    {
        yield return new WaitForSeconds(0.25f);
        if(!boss.activeSelf)
            boss.SetActive(true);
    }
}
