using UnityEngine;

public abstract class AbilitySO : ScriptableObject {

    public string abilityName = "New Ability";
    public Sprite sprite;
    public float fireRate = 5f;
    public float range = 100f;
    public float damage = 10f;


    public abstract void Initialize(GameObject gameObject);
    public abstract void Fire();

    


}
