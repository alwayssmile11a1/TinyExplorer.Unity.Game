using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireFlyController : MonoBehaviour {

    public GameObject bullet;
    public Transform startShootPosition;
    public float fireRate = 5f;

    // Use this for initialization
    void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void Trigger(Vector3 position)
    {

        GameObject cloneBullet = Instantiate(bullet, startShootPosition.position, startShootPosition.rotation);
        //cloneBullet.GetComponent<Rigidbody2D>().velocity = 5f;

        Destroy(cloneBullet, 2f);

    }




}
