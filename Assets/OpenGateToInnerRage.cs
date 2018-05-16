using Gamekit2D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenGateToInnerRage : MonoBehaviour {
    public Sprite changeImage;
    public MovingGround innerRageDoor;
    [Range(0, 1)]
    public float shakeAmount;
    [Range(1, 5)]
    public float timeCameraShake;
    private BoxCollider2D boxCollider2D;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void OnHit()
    {
        Debug.Log("hit");
        Destroy(boxCollider2D);
        spriteRenderer.sprite = changeImage;
        StartCoroutine(OpenGate());
    }

    IEnumerator OpenGate()
    {
        yield return new WaitForSeconds(0.3f);
        innerRageDoor.CanMove = true;
        CameraShaker.Shake(shakeAmount, timeCameraShake);
    }
}
