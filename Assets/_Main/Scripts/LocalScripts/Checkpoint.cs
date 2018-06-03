using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamekit2D;

[RequireComponent(typeof(BoxCollider2D))]
public class Checkpoint : MonoBehaviour, IDataPersister
{
    public string checkPointHitEffectName;
    public bool respawnFacingLeft;

    [Tooltip("Useful in boss fight")]
    public bool forceResetGame;
    [HideInInspector]
    public DataSettings dataSettings;

    private int m_CheckPointEffectHash1;
    private int m_CheckPointEffectHash2;

    private void Awake()
    {
        m_CheckPointEffectHash1 = VFXController.StringToHash(checkPointHitEffectName);
        m_CheckPointEffectHash2 = VFXController.StringToHash("CFX_ExplosionCyan");
    }

    private void Reset()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        AlessiaController alessia = collision.GetComponent<AlessiaController>();

        if (alessia != null)
        {
            alessia.SetChekpoint(this);
        }
        VFXController.Instance.Trigger(m_CheckPointEffectHash1, transform.position, 0, false, null, null);
        VFXController.Instance.Trigger(m_CheckPointEffectHash2, transform.position, 0, false, null, null);
    }

    public void SetForceResetGame(bool forceResetGame)
    {
        this.forceResetGame = forceResetGame;
        PersistentDataManager.SetDirty(this);
    }

    public DataSettings GetDataSettings()
    {
        return dataSettings;
    }

    public void SetPersistenceDataSettings(string dataTag, DataSettings.PersistenceType persistenceType)
    {
        dataSettings.dataTag = dataTag;
        dataSettings.persistenceType = persistenceType;
    }

    public Data SavePersistenceData()
    {
        return new Data<bool>(forceResetGame);
    }

    public void LoadPersistenceData(Data data)
    {
        Data<bool> directorTriggerData = (Data<bool>)data;
        forceResetGame = directorTriggerData.value;
    }

}
