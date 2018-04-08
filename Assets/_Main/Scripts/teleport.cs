using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class teleport : MonoBehaviour {
    public Transform target;
    public Transform destination;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(target.position);
        target.position = destination.position;
        Debug.Log(target.position);
    }
}
