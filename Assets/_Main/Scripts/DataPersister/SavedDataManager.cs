using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;

namespace Gamekit2D
{
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
        protected Dictionary<string, Vector3> m_Vector3SavedData = new Dictionary<string, Vector3>();


        protected HashSet<IDataSaveable> m_DataSaveables = new HashSet<IDataSaveable>();


        private bool m_DataLoaded;

        void Start()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            //else
            //{
            //    DontDestroyOnLoad(gameObject);
            //}

        }

        public static string GetString(string key)
        {
            string value;
            if (Instance.m_StringSavedData.TryGetValue(key, out value))
                return value;
            throw new UnityException("No string data was found with the key - " + key);
        }

        public static bool GetBool(string key)
        {
            bool value;

            if (Instance.m_BoolSavedData.TryGetValue(key, out value))
                return value;
            throw new UnityException("No bool data was found with the key - " + key);
        }

        public static int GetInt(string key)
        {
            int value;
            if (Instance.m_IntSavedData.TryGetValue(key, out value))
                return value;
            throw new UnityException("No int data was found with the key - " + key);
        }

        public static float GetFloat(string key)
        {
            float value;
            if (Instance.m_FloatSavedData.TryGetValue(key, out value))
                return value;
            throw new UnityException("No float data was found with the key - " + key);
        }

        public static Vector3 GetVector3(string key)
        {
            Vector3 value;
            if (Instance.m_Vector3SavedData.TryGetValue(key, out value))
                return value;
            throw new UnityException("No Vector3 data was found with the key - " + key);
        }



        public static void Set(string key, string value)
        {
            if (Instance.m_StringSavedData.ContainsKey(key))
                Instance.m_StringSavedData[key] = value;
            else
                Instance.m_StringSavedData.Add(key, value);
        }

        public static void Set(string key, bool value)
        {
            if (Instance.m_BoolSavedData.ContainsKey(key))
                Instance.m_BoolSavedData[key] = value;
            else
                Instance.m_BoolSavedData.Add(key, value);
        }

        public static void Set(string key, int value)
        {
            if (Instance.m_IntSavedData.ContainsKey(key))
                Instance.m_IntSavedData[key] = value;
            else
                Instance.m_IntSavedData.Add(key, value);
        }

        public static void Set(string key, float value)
        {
            if (Instance.m_FloatSavedData.ContainsKey(key))
                Instance.m_FloatSavedData[key] = value;
            else
                Instance.m_FloatSavedData.Add(key, value);
        }

        public static void Set(string key, Vector2 value)
        {
            if (Instance.m_Vector3SavedData.ContainsKey(key))
                Instance.m_Vector3SavedData[key] = value;
            else
                Instance.m_Vector3SavedData.Add(key, value);
        }



        public static void SaveAllData()
        {
            //Get scene name
            string sceneName = SceneManager.GetActiveScene().name;

            BinaryFormatter bf = new BinaryFormatter();
            string path = Path.Combine(Application.persistentDataPath, sceneName + "TinyExplorer.dat");
            FileStream file = File.Create(path);

            //Since Unity doesn't support serializing Dictionary, we have to convert it to List 
            SavedData savedData = new SavedData();


            savedData.stringKeys = new List<string>(Instance.m_StringSavedData.Keys);
            savedData.stringData = new List<string>(Instance.m_StringSavedData.Values);

            savedData.intKeys = new List<string>(Instance.m_IntSavedData.Keys);
            savedData.intData = new List<int>(Instance.m_IntSavedData.Values);

            savedData.floatKeys = new List<string>(Instance.m_FloatSavedData.Keys);
            savedData.floatData = new List<float>(Instance.m_FloatSavedData.Values);

            savedData.boolKeys = new List<string>(Instance.m_BoolSavedData.Keys);
            savedData.boolData = new List<bool>(Instance.m_BoolSavedData.Values);

            savedData.vectorKeys = new List<string>(Instance.m_Vector3SavedData.Keys);
            savedData.vectorData = new List<Vector3>(Instance.m_Vector3SavedData.Values);


            //QuickSavedData[] quickSavedDataArray = FindObjectsOfType<QuickSavedData>();
            //for (int i = 0; i < quickSavedDataArray.Length; i++)
            //{
            //    if (quickSavedDataArray[i].saveActiveState)
            //    {
            //        savedData.boolKeys.Add(quickSavedDataArray[i].savedDataTag);
            //        savedData.boolData.Add(quickSavedDataArray[i].gameObject.activeSelf);
            //    }
            //}

            foreach(IDataSaveable iDataSaveable in Instance.m_DataSaveables)
            {
                iDataSaveable.SaveData();
            }


            bf.Serialize(file, savedData);

            file.Close();


        }

        /// <summary>
        /// Load data of current scene and return true if data is successfully loaded or has already been loaded
        /// </summary>
        /// <returns></returns>
        public static bool LoadAllData()
        {

            if (Instance.m_DataLoaded) return true;

            //Get scene name
            string sceneName = SceneManager.GetActiveScene().name;
            string path = Path.Combine(Application.persistentDataPath, sceneName + "TinyExplorer.dat");

            if (File.Exists(path))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(path, FileMode.Open);

                SavedData savedData = (SavedData)bf.Deserialize(file);

                //String data
                Instance.m_StringSavedData = new Dictionary<string, string>();
                for (int i = 0; i < savedData.stringKeys.Count; i++)
                {
                    Instance.m_StringSavedData.Add(savedData.stringKeys[i], savedData.stringData[i]);
                }

                //Int data
                Instance.m_IntSavedData = new Dictionary<string, int>();
                for (int i = 0; i < savedData.intKeys.Count; i++)
                {
                    Instance.m_IntSavedData.Add(savedData.intKeys[i], savedData.intData[i]);
                }

                //Float data
                Instance.m_FloatSavedData = new Dictionary<string, float>();
                for (int i = 0; i < savedData.floatKeys.Count; i++)
                {
                    Instance.m_FloatSavedData.Add(savedData.floatKeys[i], savedData.floatData[i]);
                }

                //Bool data
                Instance.m_BoolSavedData = new Dictionary<string, bool>();
                for (int i = 0; i < savedData.boolKeys.Count; i++)
                {
                    Instance.m_BoolSavedData.Add(savedData.boolKeys[i], savedData.boolData[i]);
                }

                //Vector data
                Instance.m_Vector3SavedData = new Dictionary<string, Vector3>();
                for (int i = 0; i < savedData.vectorKeys.Count; i++)
                {
                    Instance.m_Vector3SavedData.Add(savedData.vectorKeys[i], savedData.vectorData[i]);
                }

                //QuickSavedData[] quickSavedDataArray = FindObjectsOfType<QuickSavedData>();

                //for (int i = 0; i < quickSavedDataArray.Length; i++)
                //{
                //    if (quickSavedDataArray[i].saveActiveState)
                //    {
                //        quickSavedDataArray[i].gameObject.SetActive(Instance.m_BoolSavedData[quickSavedDataArray[i].savedDataTag]);
                //    }
                //}

                foreach (IDataSaveable iDataSaveable in Instance.m_DataSaveables)
                {
                    iDataSaveable.LoadData();
                }

                Instance.m_DataLoaded = true;

                file.Close();

                return true;
            }


            return false;
        }


        public static void Register(IDataSaveable iDataSaveable)
        {
            Instance.m_DataSaveables.Add(iDataSaveable);
        }

        public static void Unregister(IDataSaveable iDataSaveable)
        {
            Instance.m_DataSaveables.Remove(iDataSaveable);
        }


    }
}