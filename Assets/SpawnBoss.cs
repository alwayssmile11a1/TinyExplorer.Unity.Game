using Gamekit2D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SpawnBoss : MonoBehaviour {
    public GameObject boss;
    public GameObject AppearEffect;
    //public BackgroundMusicPlayer backgroundMusicPlayer;
    public UnityEvent OnSpawnBoss;
    public ParticleSystem Effect;
    [Range(0, 3)]
    public float waitAmountToSpawnBoss;
    private BoxCollider2D boxCollider2D;
    private void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnSpawnBoss.Invoke();
        if (collision.tag.Equals("Player"))
        {
            Effect.Stop();
            StartCoroutine(Spawn());
            Destroy(boxCollider2D);
        }
    }

    private IEnumerator Spawn()
    {
        yield return new WaitForSeconds(waitAmountToSpawnBoss);
        if (AppearEffect != null && !AppearEffect.activeSelf)
            AppearEffect.SetActive(true);
        yield return new WaitForSeconds(0.25f);
        if (!boss.activeSelf)
            boss.SetActive(true);
    }
}
