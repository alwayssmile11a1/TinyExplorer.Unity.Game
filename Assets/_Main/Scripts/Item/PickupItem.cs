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
        public string pickupEffect = "";
        public AudioClip clip;
        public UnityEngine.Events.UnityEvent onTriggerEnter;

        public DataSettings dataSettings;


        [HideInInspector]
        new public Collider2D collider;

        private int m_HashPickupEffect;

        private void Awake()
        {
            m_HashPickupEffect = VFXController.StringToHash(pickupEffect);
        }

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

                VFXController.Instance.Trigger(m_HashPickupEffect, transform.position, 0, false, null);
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

        public void SetPersistenceDataSettings(string dataTag, DataSettings.PersistenceType persistenceType)
        {
            dataSettings.dataTag = dataTag;
            dataSettings.persistenceType = persistenceType;
        }

        public Data SavePersistenceData()
        {
            return new Data<bool>(gameObject.activeSelf);
        }

        public void LoadPersistenceData(Data data)
        {
            Data<bool> savedData = (Data<bool>)data;
            gameObject.SetActive(savedData.value);
        }

        //public void OnReset()
        //{
        //    gameObject.SetActive(true);
        //    Save();
        //}
    }
}