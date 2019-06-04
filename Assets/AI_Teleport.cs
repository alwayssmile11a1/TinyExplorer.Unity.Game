using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Teleport : MonoBehaviour
{
    public Transform destination;
    public bool flipAfterTele;
    public bool stopAfterTeleport;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("ZombieAI"))
        {
            collision.transform.position = destination.position;

            if (flipAfterTele)
            {
                if (collision.transform.localRotation.y == 0)
                    collision.transform.localRotation = Quaternion.Euler(0, 180, 0);
                else
                    collision.transform.localRotation = Quaternion.Euler(0, 0, 0);
                collision.transform.GetComponent<ZombieKnight_AI_Behaviour>().isFlip = collision.transform.localRotation.y == 0 ? false : true;
            }

            if (stopAfterTeleport)
            {
                collision.transform.GetComponent<ZombieKnight_AI_Behaviour>().stop = true;
                collision.transform.GetComponent<ZombieKnight_AI_Behaviour>().RunAI(new float[] { 0, 0 });
            }
        }
    }
}
