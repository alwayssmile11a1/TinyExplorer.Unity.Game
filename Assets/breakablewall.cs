using Gamekit2D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class breakablewall : MonoBehaviour {

    int Hash = VFXController.StringToHash("CFX2_RockHit");
    
    public void OnDie()
    {
        Debug.Log("Break");
        VFXController.Instance.Trigger(Hash, transform.position, 0, false, null, null);
        Destroy(gameObject);
    }
}
