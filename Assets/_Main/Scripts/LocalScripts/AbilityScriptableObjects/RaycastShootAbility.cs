using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Ability/RaycastShootAbility", order = 1)]
public class RaycastShootAbility : AbilityScriptableObject {

    //[Tooltip("set to 0 or a negative number if ")]
    public float fireRate = 5f;
    public Gradient laserColor;


    private RaycastShootTriggerable rcShooter;


    public override void Initialize(GameObject gameObject)
    {
        rcShooter = gameObject.GetComponent<RaycastShootTriggerable>();

        if (rcShooter == null) return;

        rcShooter.range = range;
        rcShooter.laserColor = laserColor;
        rcShooter.fireRate = fireRate;

        rcShooter.Initialize();

    }
    public override void Trigger()
    {
        if (rcShooter != null)
        {
            rcShooter.Trigger();
        }
    }

}
