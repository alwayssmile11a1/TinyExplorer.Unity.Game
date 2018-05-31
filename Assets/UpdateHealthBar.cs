using Gamekit2D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateHealthBar : MonoBehaviour {
    private Damageable damageable;
    private float lostHealthAmount;

    private RectTransform rectTransform;
	// Use this for initialization
	void Start () {
        damageable = GetComponentInParent<Damageable>();
        rectTransform = GetComponent<RectTransform>();
        lostHealthAmount = rectTransform.sizeDelta.x / damageable.startingHealth;
	}

    public void ChangeWidth()
    {
        Debug.Log(rectTransform.sizeDelta.x);
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x - lostHealthAmount, rectTransform.sizeDelta.y);
    }
}
