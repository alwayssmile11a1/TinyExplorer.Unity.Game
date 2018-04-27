using UnityEngine;

public abstract class AbilityScriptableObject : ScriptableObject {

    public string abilityName = "New Ability";

    [Tooltip("used in case you wanna display the ability sprite to UI")]
    public Sprite sprite;
    public float range = 100f;


    public abstract void Initialize(GameObject gameObject);
    public abstract void Trigger();

    


}
