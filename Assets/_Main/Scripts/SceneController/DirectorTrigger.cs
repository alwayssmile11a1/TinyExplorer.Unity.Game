using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

namespace Gamekit2D
{
    [RequireComponent(typeof(Collider2D))]
    public class DirectorTrigger : MonoBehaviour, IDataPersister
    {
        public enum TriggerType
        {
            Once, Everytime,
        }

        [Tooltip("This is the layers which will trigger the director to play.  For example, the player.")]
        public LayerMask targetLayers;
        public PlayableDirector director;
        public TriggerType triggerType;
        public UnityEvent OnDirectorPlay;
        public UnityEvent OnDirectorFinish;
        [HideInInspector]
        public DataSettings dataSettings;

        protected bool m_AlreadyTriggered;

        void OnEnable()
        {
            PersistentDataManager.RegisterPersister(this);
        }

        void OnDisable()
        {
            PersistentDataManager.UnregisterPersister(this);
        }

        void OnTriggerEnter2D(Collider2D other)
        {

            if (!targetLayers.Contains(other.gameObject))
                return;


            if (triggerType == TriggerType.Once && m_AlreadyTriggered)
                return;
            
            director.Play();
            m_AlreadyTriggered = true;
            PersistentDataManager.SetDirty(this);
            OnDirectorPlay.Invoke();
            Invoke("FinishInvoke", (float)director.duration);
        }

        void FinishInvoke()
        {
            OnDirectorFinish.Invoke();
        }

        public void OverrideAlreadyTriggered(bool alreadyTriggered)
        {
            m_AlreadyTriggered = alreadyTriggered;
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
            return new Data<bool>(m_AlreadyTriggered);
        }

        public void LoadPersistenceData(Data data)
        {
            Data<bool> directorTriggerData = (Data<bool>)data;
            m_AlreadyTriggered = directorTriggerData.value;
        }

        //public void OnReset()
        //{
        //    m_AlreadyTriggered = false;
        //    PersistentDataManager.SetDirty(this);
            

        //}
    }
}