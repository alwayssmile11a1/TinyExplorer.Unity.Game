using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Teleport : MonoBehaviour
{
    public Transform destination;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("ZombieAI"))
        {
            collision.transform.position = destination.position;
        }
    }
}
