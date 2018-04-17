using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerRmx6BT : MonoBehaviour {
    private rmx6BT rmx6BT;
    private void Awake()
    {
        rmx6BT = GetComponentInChildren<rmx6BT>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Player"))
        {
            rmx6BT.targetInRange = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag.Equals("Player"))
        {
            rmx6BT.targetInRange = false;
        }
    }
}
