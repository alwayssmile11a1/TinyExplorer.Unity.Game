using Gamekit2D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class breakablewall : MonoBehaviour {

    public string effectName = "CFX2_RockHit";

    int Hash;

    private void Awake()
    {
        Hash = VFXController.StringToHash(effectName);
    }

    public void OnDie()
    {
        VFXController.Instance.Trigger(Hash, transform.position, 0, false, null, null);
        gameObject.SetActive(false);
    }
}
