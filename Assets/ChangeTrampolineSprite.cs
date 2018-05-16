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

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D targetRigidbody2D;


    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
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
            currentCooldown = jumpCoolDown;
            StartCoroutine(SetTramPolineImage());
        }

    }

    IEnumerator SetTramPolineImage()
    {
        yield return new WaitForSeconds(jumpCoolDown);
        spriteRenderer.sprite = nonJumpedTranpoline;
    }
}
