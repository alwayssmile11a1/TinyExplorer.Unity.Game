using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeTrampolineSprite : MonoBehaviour {
    public Sprite jumpedTranpoline;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        spriteRenderer.sprite = jumpedTranpoline;
    }
}
