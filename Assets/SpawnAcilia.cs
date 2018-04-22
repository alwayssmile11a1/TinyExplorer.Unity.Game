using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAcilia : MonoBehaviour {
    public GameObject Alicia;
    public GameObject AppearEffect;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Player"))
        {
            if(AppearEffect != null && !AppearEffect.activeSelf)
                AppearEffect.SetActive(true);
            StartCoroutine(SpawnAlicia());
        }
    }

    private IEnumerator SpawnAlicia()
    {
        yield return new WaitForSeconds(0.25f);
        if(!Alicia.activeSelf)
            Alicia.SetActive(true);
    }
}
