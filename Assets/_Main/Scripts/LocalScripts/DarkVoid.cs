using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamekit2D;
public class DarkVoid : MonoBehaviour {

    public GameObject darkMatter;

    private BulletPool m_DarkMatterPool;

	// Use this for initialization
	void Awake () {
        m_DarkMatterPool = BulletPool.GetObjectPool(darkMatter, 2);

    }

    public void Spread()
    {
        BulletObject darkMatter1 = m_DarkMatterPool.Pop(transform.position);
        BulletObject darkMatter2 = m_DarkMatterPool.Pop(transform.position);

        darkMatter1.rigidbody2D.velocity = 5f * Vector2.left;
        darkMatter2.rigidbody2D.velocity = 5f * Vector2.right;
    }

    public void OnNonDamagableHit(Damager damager)
    {
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }

}
