using Gamekit2D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeBackgroundMusic : MonoBehaviour {
    public BackgroundMusicPlayer backgroundMusicPlayer;
    public AudioClip clipToChange;
    // Use this for initialization
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Trigger");
        
        //if (clipToChange)
        //{
        //    StartCoroutine(PlayBossFightMusic());
        //}
    }

    IEnumerator PlayBossFightMusic()
    {
        Debug.Log("asdasdasd");
        yield return new WaitForSeconds(1f);
        //AudioSource.PlayClipAtPoint(clipToChange, transform.position);
        backgroundMusicPlayer.ChangeMusic(clipToChange);
    }
}
