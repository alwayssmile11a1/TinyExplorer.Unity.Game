using Gamekit2D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class teleport : MonoBehaviour {
    public Transform target;
    public Transform destination;
    public ActiveBound activeBound;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Player"))
        {
            if(!(activeBound is null))
            {
                activeBound.followPointChangingSmoothSpeed = 10;
            }
            collision.transform.position = destination.position;
        }
    }

    IEnumerator ResetCameraSmoothSpeed()
    {
        yield return new WaitForSeconds(1f);
        activeBound.followPointChangingSmoothSpeed = 1;
    }
}
