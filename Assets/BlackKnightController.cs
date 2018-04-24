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
    public GameObject attack3Bullet;
    public Transform movePos;
    public Rigidbody2D parentRigidbody;
    public float moveSpeed;
    public ParticleSystem Flying_Upward_Effect;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    int turn;
    BulletPool bulletPool;
    BulletPool attack3BulletPool;

    Root BlackKnightBT = BT.Root();
	// Use this for initialization
	void Start () {
        bulletPool = BulletPool.GetObjectPool(bullet, 5);
        attack3BulletPool = BulletPool.GetObjectPool(attack3Bullet, 8);
        bulletObjects = new BulletObject[shootPoints.Length];
        timeToCoolDown = 0;
        currentAmount = 0;
        //turn = 1;

        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

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
            BT.If(() => turn++ == 3).OpenBranch(
                BT.Sequence().OpenBranch(
                    BT.Wait(2f),
                    BT.Call(() => Flying_Upward_Effect.Play()),
                    BT.Wait(0.5f),
                    BT.SetBool(animator, "move", true),
                    BT.WaitForAnimatorState(animator, "move"),
                    BT.Call(MoveToNewPos),
                    BT.WaitUntil(MoveCheck),
                    BT.SetBool(animator, "move", false),
                    BT.Wait(1f),
                    BT.Call(() => attack3Bullet.SetActive(true)),
                    BT.Wait(3f),
                    BT.Call(BulletFollowTarget)
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
        Vector2 direction = (movePos.position - transform.position).normalized;
        parentRigidbody.velocity = direction * moveSpeed;
    }

    private bool MoveCheck()
    {
        if ((movePos.position - transform.position).sqrMagnitude <= 0.5f)
        {
            parentRigidbody.velocity = new Vector2(0, 0);
            return true;
        }
        return false;
    }

    private void BulletFollowTarget()
    {
        attack3Bullet.GetComponent<FollowTarget>().enabled = true;
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
