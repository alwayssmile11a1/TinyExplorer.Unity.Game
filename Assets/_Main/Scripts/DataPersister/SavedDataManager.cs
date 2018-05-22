using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;

namespace Gamekit2D
{
    /// <summary>
    /// A class to help easily save data such as player state, level state.
    /// Note that this class is intended to save only ISaveable Data of the current scene, any other custom data need to be save through SaveCustomData function or their own Save and Load function.
    /// </summary>
    public class SavedDataManager : MonoBehaviour
    {
        public static SavedDataManager Instance
        {
            get
            {
                if (instance != null)
                    return instance;

                instance = FindObjectOfType<SavedDataManager>();

                if (instance != null)
                    return instance;

                GameObject spawnManagerGameObject = new GameObject("SavedDataManager");
                instance = spawnManagerGameObject.AddComponent<SavedDataManager>();

                return instance;
            }
        }

        protected static SavedDataManager instance;

        protected Dictionary<string, string> m_StringSavedData = new Dictionary<string, string>();
        protected Dictionary<string, bool> m_BoolSavedData = new Dictionary<string, bool>();
        protected Dictionary<string, int> m_IntSavedData = new Dictionary<string, int>();
        protected Dictionary<string, float> m_FloatSavedData = new Dictionary<string, float>();
        protected Dictionary<string, SerializableVector3> m_Vector3SavedData = new Dictionary<string, SerializableVector3>();


        protected HashSet<IDataSaveable> m_DataSaveables = new HashSet<IDataSaveable>();


        private bool m_DataLoaded;

        void Start()
        {
            if (Instance != this)
            {
                Destroy(gameObject);
            }

        }

        public string GetString(string key)
        {
            string value;
            if (m_StringSavedData.TryGetValue(key, out value))
                return value;
            throw new UnityException("No string data was found with the key - " + key);
        }

        public bool GetBool(string key)
        {
            bool value;

            if (m_BoolSavedData.TryGetValue(key, out value))
                return value;
            throw new UnityException("No bool data was found with the key - " + key);
        }

        public int GetInt(string key)
        {
            int value;
            if (m_IntSavedData.TryGetValue(key, out value))
                return value;
            throw new UnityException("No int data was found with the key - " + key);
        }

        public float GetFloat(string key)
        {
            float value;
            if (m_FloatSavedData.TryGetValue(key, out value))
                return value;
            throw new UnityException("No float data was found with the key - " + key);
        }

        public Vector3 GetVector3(string key)
        {
            SerializableVector3 value;
            if (m_Vector3SavedData.TryGetValue(key, out value))
                return new Vector3(value.x,value.y,value.z);
            throw new UnityException("No Vector3 data was found with the key - " + key);
        }



        public void Set(string key, string value)
        {
            if (m_StringSavedData.ContainsKey(key))
                m_StringSavedData[key] = value;
            else
                m_StringSavedData.Add(key, value);
        }

        public void Set(string key, bool value)
        {
            if (m_BoolSavedData.ContainsKey(key))
                m_BoolSavedData[key] = value;
            else
                m_BoolSavedData.Add(key, value);
        }

        public void Set(string key, int value)
        {
            if (m_IntSavedData.ContainsKey(key))
                m_IntSavedData[key] = value;
            else
                m_IntSavedData.Add(key, value);
        }

        public void Set(string key, float value)
        {
            if (m_FloatSavedData.ContainsKey(key))
                m_FloatSavedData[key] = value;
            else
                m_FloatSavedData.Add(key, value);
        }

        public void Set(string key, Vector3 value)
        {
            SerializableVector3 serializableVector3 = new SerializableVector3();
            serializableVector3.x = value.x;
            serializableVector3.y = value.y;
            serializableVector3.z = value.z;

            if (m_Vector3SavedData.ContainsKey(key))
            {
                m_Vector3SavedData[key] = serializableVector3;
            }
            else
            {
                m_Vector3SavedData.Add(key, serializableVector3);
            }
        }



        public void SaveCustomData(string addtionalPath, SavedData savedData)
        {
            
            BinaryFormatter bf = new BinaryFormatter();
            string path = Path.Combine(Application.persistentDataPath, addtionalPath + "TinyExplorer.dat");
            FileStream file = File.Create(path);

            //Since Unity doesn't support serializing Dictionary, we have to convert it to List 
            SerializableSavedData serializableData = savedData.GetSeralizableData();

            bf.Serialize(file, serializableData);

            file.Close();


        }

        /// <summary>
        /// Load custom data and return true 
        /// </summary>
        /// <returns></returns>
        public SavedData LoadCustomData(string addtionalPath)
        {
            string path = Path.Combine(Application.persistentDataPath, addtionalPath + "TinyExplorer.dat");

            if (File.Exists(path))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(path, FileMode.Open);

                SerializableSavedData serializableData = (SerializableSavedData)bf.Deserialize(file);

                SavedData savedData = new SavedData();
                savedData.FromSerializableData(serializableData);
              
                file.Close();

                return savedData;
            }

            return null;
        }

        /// <summary>
        /// Save data of the gameObjects which implemented and registed IDataSaveable interface
        /// </summary>
        public void SaveSceneData()
        {
            string sceneName = SceneManager.GetActiveScene().name;

            BinaryFormatter bf = new BinaryFormatter();
            string path = Path.Combine(Application.persistentDataPath, sceneName + "TinyExplorer.dat");
            FileStream file = File.Create(path);

            foreach (IDataSaveable iDataSaveable in m_DataSaveables)
            {
                iDataSaveable.SaveData();
            }

            //Since Unity doesn't support serializing Dictionary, we have to convert it to List 
            SerializableSavedData savedData = new SerializableSavedData();

            savedData.stringKeys = new List<string>(m_StringSavedData.Keys);
            savedData.stringData = new List<string>(m_StringSavedData.Values);

            savedData.intKeys = new List<string>(m_IntSavedData.Keys);
            savedData.intData = new List<int>(m_IntSavedData.Values);

            savedData.floatKeys = new List<string>(m_FloatSavedData.Keys);
            savedData.floatData = new List<float>(m_FloatSavedData.Values);

            savedData.boolKeys = new List<string>(m_BoolSavedData.Keys);
            savedData.boolData = new List<bool>(m_BoolSavedData.Values);

            savedData.vectorKeys = new List<string>(m_Vector3SavedData.Keys);
            savedData.vectorData = new List<SerializableVector3>(m_Vector3SavedData.Values);

            bf.Serialize(file, savedData);

            file.Close();


        }

        /// <summary>
        /// Load data of current scene and return true if data is successfully loaded or has already been loaded
        /// </summary>
        /// <returns></returns>
        public bool LoadSceneData()
        {

            if (m_DataLoaded) return true;

            string sceneName = SceneManager.GetActiveScene().name;

            string path = Path.Combine(Application.persistentDataPath, sceneName + "TinyExplorer.dat");

            if (File.Exists(path))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(path, FileMode.Open);

                SerializableSavedData savedData = (SerializableSavedData)bf.Deserialize(file);

                //String data
                m_StringSavedData = new Dictionary<string, string>();
                for (int i = 0; i < savedData.stringKeys.Count; i++)
                {
                    m_StringSavedData.Add(savedData.stringKeys[i], savedData.stringData[i]);
                }

                //Int data
                m_IntSavedData = new Dictionary<string, int>();
                for (int i = 0; i < savedData.intKeys.Count; i++)
                {
                    m_IntSavedData.Add(savedData.intKeys[i], savedData.intData[i]);
                }

                //Float data
                m_FloatSavedData = new Dictionary<string, float>();
                for (int i = 0; i < savedData.floatKeys.Count; i++)
                {
                    m_FloatSavedData.Add(savedData.floatKeys[i], savedData.floatData[i]);
                }

                //Bool data
                m_BoolSavedData = new Dictionary<string, bool>();
                for (int i = 0; i < savedData.boolKeys.Count; i++)
                {
                    m_BoolSavedData.Add(savedData.boolKeys[i], savedData.boolData[i]);
                }

                //Vector data
                m_Vector3SavedData = new Dictionary<string, SerializableVector3>();
                for (int i = 0; i < savedData.vectorKeys.Count; i++)
                {
                    m_Vector3SavedData.Add(savedData.vectorKeys[i], savedData.vectorData[i]);
                }

                foreach (IDataSaveable iDataSaveable in m_DataSaveables)
                {
                    iDataSaveable.LoadData();
                }

                m_DataLoaded = true;

                file.Close();

                return true;
            }


            return false;
        }

        /// <summary>
        /// Delete data
        /// </summary>
        /// <param name="additionalPath">Can be scene name or custom name</param>
        public void DeleteData(string additionalPath)
        {
            string path = Path.Combine(Application.persistentDataPath, additionalPath + "TinyExplorer.dat");
            if (File.Exists(path))
            {
                File.Delete(path);

            }
        }

        public void Register(IDataSaveable iDataSaveable)
        {
            m_DataSaveables.Add(iDataSaveable);
        }

        public void Unregister(IDataSaveable iDataSaveable)
        {
            m_DataSaveables.Remove(iDataSaveable);
        }


    }
}

