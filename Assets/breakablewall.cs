using Gamekit2D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class breakablewall : MonoBehaviour {
    public Transform breakPos;
    public void OnDie()
    {
        Debug.Log("Break");
        GameObject.Destroy(this);
        VFXController.Instance.Trigger("CFX2_RockHit", breakPos.position, 0, false, null, null);
    }
}
