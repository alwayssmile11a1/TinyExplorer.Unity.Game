using Gamekit2D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class OpenGateToInnerRage : MonoBehaviour {
    public Sprite changeImage;
    public MovingGround innerRageDoor;
    [Range(0, 1)]
    public float shakeAmount;
    [Range(1, 5)]
    public float timeCameraShake;
    public float timeLine;

    private BoxCollider2D boxCollider2D;
    private SpriteRenderer spriteRenderer;
    private PlayableDirector playableDirector;

    private void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playableDirector = GetComponent<PlayableDirector>();
    }

    public void OnHit()
    {
        Debug.Log("hit");
        Destroy(boxCollider2D);
        spriteRenderer.sprite = changeImage;
        playableDirector.Play();
        StartCoroutine(OpenGate());
    }

    IEnumerator OpenGate()
    {
        yield return new WaitForSeconds(timeLine);
        innerRageDoor.CanMove = true;
        CameraShaker.Shake(shakeAmount, timeCameraShake);
    }
}
