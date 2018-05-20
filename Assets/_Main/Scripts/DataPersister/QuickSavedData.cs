using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamekit2D;

public class QuickSavedData : MonoBehaviour, IDataSaveable {

    public bool saveActiveState = true;

    [Tooltip("This is the unique id of this gameObject, so you shouldn't touch this")]
    public string savedDataTag = System.Guid.NewGuid().ToString();

    private void Awake()
    {
        SavedDataManager.Register(this);
    }


    public void LoadData()
    {
        if(saveActiveState)
        {
            gameObject.SetActive(SavedDataManager.GetBool(savedDataTag));
        }
    }

    public void SaveData()
    {
        if(saveActiveState)
        {
            SavedDataManager.Set(savedDataTag, gameObject.activeSelf);
        }
    }
}
