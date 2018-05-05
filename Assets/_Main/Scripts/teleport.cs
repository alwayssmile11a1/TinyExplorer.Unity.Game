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
            activeBound.followPointChangingSmoothSpeed = 5;
            Debug.Log(target.position);
            target.position = destination.position;
            Debug.Log(target.position);
        }
    }

    IEnumerator ResetCameraSmoothSpeed()
    {
        yield return new WaitForSeconds(1f);
        activeBound.followPointChangingSmoothSpeed = 1;
    }
}
