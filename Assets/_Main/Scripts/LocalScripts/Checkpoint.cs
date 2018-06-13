using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamekit2D;

[RequireComponent(typeof(BoxCollider2D))]
public class Checkpoint : MonoBehaviour, IDataPersister
{
    public string checkPointHitEffectName = "Blue_Explosion2";
    public bool respawnFacingLeft;

    [Tooltip("Useful in boss fight")]
    public bool forceResetGame;
    [HideInInspector]
    public DataSettings dataSettings;

    private int m_CheckPointEffectHash;
    private int m_Health;

    private void Awake()
    {
        m_CheckPointEffectHash = VFXController.StringToHash(checkPointHitEffectName);
      
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
            m_Health = alessia.GetComponent<Damageable>().CurrentHealth;

            alessia.SetChekpoint(this);

            if (m_CheckPointEffectHash != 0)
            {
                VFXController.Instance.Trigger(m_CheckPointEffectHash, transform.position, 0, false, null, null);
                m_CheckPointEffectHash = 0;
            }
        }
    }

    public int GetHealth()
    {
        return m_Health;
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
