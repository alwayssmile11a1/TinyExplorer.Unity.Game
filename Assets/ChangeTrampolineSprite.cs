using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeTrampolineSprite : MonoBehaviour {
    public Sprite jumpedTranpoline;
    public Sprite nonJumpedTranpoline;
    public float thrust;
    public float jumpCoolDown;

    private float currentCooldown;
    private bool canJumped = false;
    private Vector2 offset = new Vector2(0, -0.04f);
    private Vector2 sizeBeforeJump = new Vector2(0.44f, 0.25f);
    private Vector2 sizeAfterJump = new Vector2(0.44f, 0.32f);

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D targetRigidbody2D;
    private BoxCollider2D boxCollider2D;


    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider2D = GetComponent<BoxCollider2D>();
    }
    private void Update()
    {
        if(currentCooldown >= 0)
        {
            currentCooldown -= Time.deltaTime;
        }
    }
    private void FixedUpdate()
    {
        if(canJumped)
        {
            Debug.Log("Rigidbody: " + targetRigidbody2D.tag);
            targetRigidbody2D.AddForce(Vector2.up * thrust, ForceMode2D.Impulse);
            canJumped = false;
        }
    }

    //private

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(!canJumped && currentCooldown <= 0)
        {
            Debug.Log("Oncollisionenter");
            spriteRenderer.sprite = jumpedTranpoline;
            targetRigidbody2D = collision.rigidbody;
            canJumped = true;
            boxCollider2D.offset = Vector2.zero;
            boxCollider2D.size = sizeAfterJump;
            currentCooldown = jumpCoolDown;
            StartCoroutine(SetTramPolineImage());
        }

    }

    IEnumerator SetTramPolineImage()
    {
        yield return new WaitForSeconds(jumpCoolDown);
        spriteRenderer.sprite = nonJumpedTranpoline;
        boxCollider2D.offset = offset;
        boxCollider2D.size = sizeBeforeJump;
    }
}
