using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamekit2D
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class Checkpoint : MonoBehaviour, IDataPersister
    {
        public ParticleSystem checkPointHitEffect;
        public bool respawnFacingLeft;

        [Tooltip("Useful in boss fight")]
        public bool forceResetGame;
        [HideInInspector]
        public DataSettings dataSettings;

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
            if(checkPointHitEffect != null)
            {
                checkPointHitEffect.Play();
            }
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
}