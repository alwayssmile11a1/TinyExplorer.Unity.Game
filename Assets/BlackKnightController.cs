using BTAI;
using Gamekit2D;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackKnightController : MonoBehaviour {
    public Transform targetToTrack;
    public GameObject bullet;
    public float bulletSpeed;

    [Header("Attack1")]
    public float attack1CoolDown;
    private float timeToCoolDown;
    [Range(1, 4)]
    public int amount;
    private int currentAmount;
    public ParticleSystem attack1Effect;
    public Transform[] shootPoints;
    private BulletObject[] bulletObjects;

    [Header("Attack2")]
    public GameObject alicia;
    public Transform spawnPos;
    public ParticleSystem aliciaAppearEffect;
    public Color defaultColor;
    [HideInInspector]
    public static bool aliciaDied = false;

    [Header("Attack3")]
    public Transform movePos;
    public float moveSpeed;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rigidbody2D;
    [SerializeField]
    int turn;
    BulletPool bulletPool;

    Root BlackKnightBT = BT.Root();
	// Use this for initialization
	void Start () {
        bulletPool = BulletPool.GetObjectPool(bullet, 5);
        bulletObjects = new BulletObject[shootPoints.Length];
        timeToCoolDown = 0;
        currentAmount = 0;
        //turn = 1;

        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody2D = GetComponent<Rigidbody2D>();

        BlackKnightBT.OpenBranch(
            BT.If(() => turn <= 2).OpenBranch(
                BT.Sequence().OpenBranch(
                BT.Wait(3f),
                BT.SetBool(animator, "attack1", true),
                BT.WaitForAnimatorState(animator, "attack1"),
                BT.Call(() => attack1Effect.Play()),
                BT.WaitUntil(Attack1),
                BT.Wait(1.5f),
                BT.SetBool(animator, "attack1", false),
                BT.Call(() => attack1Effect.Stop())
                ),
                BT.Sequence().OpenBranch(
                    BT.Wait(2f),
                    BT.SetBool(animator, "attack2", true),
                    BT.WaitForAnimatorState(animator, "attack2"),
                    BT.Call(ActiveAlicia),
                    BT.WaitUntil(() => aliciaDied),
                    BT.Call(DeactiveAlicia),
                    BT.SetBool(animator, "attack2", false)
                )
            ),
            BT.If(() => turn == 3).OpenBranch(
                BT.Sequence().OpenBranch(
                    BT.Wait(2f),
                    BT.Call(MoveToNewPos),
                    BT.WaitUntil(MoveCheck)
                )
            )
            //BT.Call(() => turn++)
        );
	}
    // Update is called once per frame
    void Update () {
        BlackKnightBT.Tick();
	}

    private bool Attack1()
    {
        if(timeToCoolDown <= 0)
        {
            //throw new NotImplementedException();
            for (int i = 0; i < shootPoints.Length; i++)
            {
                bulletObjects[i] = bulletPool.Pop(shootPoints[i].position);
                bulletObjects[i].instance.GetComponent<StartShooting>().speed = 0;
            }
            StartCoroutine(WaitToShoot());
            timeToCoolDown = attack1CoolDown;
            if(++currentAmount == amount)
            {
                currentAmount = 0;
                return true;
            }
        }
        else
        {
            timeToCoolDown -= Time.deltaTime;
        }

        return false;
    }

    private void MoveToNewPos()
    {
        Debug.Log("Move ");
        Vector2 direction = (movePos.position - transform.position).normalized;
        Debug.Log(rigidbody2D.position);
        rigidbody2D.position = movePos.position;
        Debug.Log(rigidbody2D.position);
        Debug.Log(rigidbody2D.tag);
    }

    private bool MoveCheck()
    {
        float test = (movePos.position - transform.position).sqrMagnitude;
        if ((movePos.position - transform.position).sqrMagnitude <= 1.2f)
        {
            Debug.Log("check true");
            rigidbody2D.velocity = new Vector2(0, 0);
            return true;
        }
        Debug.Log("check false");
        return false;
    }

    private IEnumerator WaitToShoot()
    {
        Debug.Log("before wait");
        yield return new WaitForSeconds(1f);
        Debug.Log("after wait");
        for (int i = 0; i < bulletObjects.Length; i++)
        {
            StartShooting startShooting = bulletObjects[i].instance.GetComponent<StartShooting>();
            startShooting.direction = (targetToTrack.transform.position - bulletObjects[i].instance.transform.position).normalized;
            startShooting.speed = bulletSpeed;
            Debug.Log(bulletObjects[i].instance.GetComponent<StartShooting>().speed);
        }
    }

    private void ActiveAlicia()
    {
        if(!alicia.activeSelf)
        {
            alicia.SetActive(true);
            alicia.transform.position = spawnPos.position;
            alicia.GetComponent<SpriteRenderer>().color = defaultColor;
        }
        aliciaAppearEffect.Play();
    }
    private void DeactiveAlicia()
    {
        alicia.SetActive(false);
        aliciaDied = false;
    }
}
