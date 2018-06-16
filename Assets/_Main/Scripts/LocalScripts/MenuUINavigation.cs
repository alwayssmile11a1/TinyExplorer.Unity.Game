using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamekit2D;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class MenuUINavigation : MonoBehaviour
{

    public GameObject continueButton;
    public TransitionPoint continueTransitionPoint;

    private void Awake()
    {
        SavedData savedData = new SavedData();

        if(savedData.Load("PlayerState"))
        {
            continueButton.SetActive(true);
            continueTransitionPoint.newSceneName = savedData.GetString("SceneName");
        }



    }


    public void Continue()
    {
        SavedData savedData = new SavedData();
        savedData.Set("Continue", true);
        savedData.Save("Continue");
    }

    public void NewGame()
    {
        SavedData savedData = new SavedData();
        savedData.Set("Continue", false);
        savedData.Save("Continue");
    }


    public void Quit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
		        Application.Quit();
#endif
    }





}




