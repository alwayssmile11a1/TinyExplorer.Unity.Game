using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamekit2D
{
    [RequireComponent(typeof(Collider2D))]
    public class PickupItem : MonoBehaviour, IDataPersister
    {

        public LayerMask layers;
        public bool disableOnEnter = false;

        [HideInInspector]
        new public Collider2D collider;

        public AudioClip clip;
        public DataSettings dataSettings;


        public UnityEngine.Events.UnityEvent onTriggerEnter;

        void OnEnable()
        {
            collider = GetComponent<Collider2D>();
            PersistentDataManager.RegisterPersister(this);
        }

        void OnDisable()
        {
            PersistentDataManager.UnregisterPersister(this);
        }

        //Unity callback function
        void Reset()
        {
            layers = LayerMask.NameToLayer("Everything");
            collider = GetComponent<Collider2D>();
            collider.isTrigger = true;
            dataSettings = new DataSettings();
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (layers.Contains(other.gameObject))
            {
                if (disableOnEnter)
                {
                    gameObject.SetActive(false);
                    Save();
                }

                onTriggerEnter.Invoke();

                if (clip) AudioSource.PlayClipAtPoint(clip, transform.position);

            }
        }

        public void Save()
        {
            PersistentDataManager.SetDirty(this);
        }

        public DataSettings GetDataSettings()
        {
            return dataSettings;
        }

        public void SetDataSettings(string dataTag, DataSettings.PersistenceType persistenceType)
        {
            dataSettings.dataTag = dataTag;
            dataSettings.persistenceType = persistenceType;
        }

        public Data SaveData()
        {
            return new Data<bool>(gameObject.activeSelf);
        }

        public void LoadData(Data data)
        {
            Data<bool> savedData = (Data<bool>)data;
            gameObject.SetActive(savedData.value);
        }


    }
}