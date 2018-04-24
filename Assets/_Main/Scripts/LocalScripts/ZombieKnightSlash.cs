using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamekit2D;

public class ZombieKnightSlash : MonoBehaviour {

    public Damager highestDamager;
    public Damager middleDamager;
    public Damager lowestDamager;


    public void EnableHighestDamager()
    {
        highestDamager.EnableDamage();
    }

    public void DisableHighestDamager()
    {
        highestDamager.DisableDamage();
    }

    public void EnableMiddleDamager()
    {
        middleDamager.EnableDamage();
    }

    public void DisableMiddleDamager()
    {
        middleDamager.DisableDamage();
    }

    public void EnableLowestDamager()
    {
        lowestDamager.EnableDamage();
    }

    public void DisableLowestDamager()
    {
        lowestDamager.DisableDamage();
    }


}
