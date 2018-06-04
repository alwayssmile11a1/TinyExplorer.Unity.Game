using Gamekit2D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

public class SpawnBoss : MonoBehaviour {
    public GameObject boss;
    public GameObject AppearEffect;
    //public BackgroundMusicPlayer backgroundMusicPlayer;
    public UnityEvent OnSpawnBoss;
    public ParticleSystem Effect;
    [Range(1, 10)]
    public float waitAmountToSpawnBoss;
    private BoxCollider2D boxCollider2D;
    private EdgeCollider2D [] edge;
    private void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Player"))
        {
            OnSpawnBoss.Invoke();
            Effect.Stop();
            StartCoroutine(Spawn());
            Destroy(boxCollider2D);
            edge = GetComponents<EdgeCollider2D>();
            foreach (var item in edge)
            {
                item.enabled = true;
            }
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
