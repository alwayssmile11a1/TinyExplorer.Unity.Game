using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamekit2D;

public class QuickSavedData : MonoBehaviour, IDataSaveable {

    public bool saveActiveState = true;
    public bool savePosition = false;

    [Tooltip("This is the unique id of this gameObject, so you shouldn't touch this")]
    public string savedDataTag = System.Guid.NewGuid().ToString();

    private void Awake()
    {
        SavedDataManager.Instance.Register(this);
    }


    public void LoadData()
    {
        if (saveActiveState)
        {
            gameObject.SetActive(SavedDataManager.GetBool(savedDataTag));
        }

        if(savePosition)
        {
            transform.position = SavedDataManager.GetVector3(savedDataTag);
        }
    }

    public void SaveData()
    {

        if (saveActiveState)
        {
            SavedDataManager.Set(savedDataTag, gameObject.activeSelf);
        }

        if(savePosition)
        {
            SavedDataManager.Set(savedDataTag, transform.position);
        }

    }
}
