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
    public UnityEvent CloseDoor;
    public ParticleSystem Effect;
    public PlayableDirector playable;
    [Range(1, 10)]
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
            CloseDoor.Invoke();
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

    public void Play(MovingGround movingGround)
    {
        if (playable)
        {
            Debug.Log(playable.duration);
            StartCoroutine(WaitToCloseDoor(movingGround));
            
        }
    }

    private IEnumerator WaitToCloseDoor(MovingGround movingGround)
    {
        playable.Play();
        yield return new WaitForSeconds(1f);
        CameraShaker.Shake(0.05f, 4.2f);
        yield return new WaitForSeconds(0.5f);
        movingGround.CanMove = true;
    }
}
