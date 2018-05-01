using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamekit2D
{
    [RequireComponent(typeof(Collider2D))]
    public class ActiveBound : MonoBehaviour, IDataPersister
    {

        public LayerMask targetLayers;
        public GameObject[] needToBeActiveGameObjects;

        [Header("Camera Bound")]
        [Tooltip("if set, when the target enter this bound, the cinemachine confinder will be changed to this specified bound")]
        public PolygonCollider2D cameraBound;

        [Header("Camera Follow Point")]
        public bool changeCameraFollowPoint = true;
        [Tooltip("when the target enter the bound, the camera will follow this point")]
        public Transform cameraFollowPoint;
        [Tooltip("SmoothSpeed")]
        public float followPointChangingSmoothSpeed = 5f;

        [Header("Orthographic Size")]
        public bool changeOrthographicSize = false;
        public float orthographicSize = 3.5f;
        [Tooltip("SmoothSpeed")]
        public float orthoSizeChangingSmoothSpeed = 5f;

        [Space(7)]
        public bool disableBoundOnExit = false;

        [Header("Events")]
        public UnityEngine.Events.UnityEvent OnEnterActiveBound;
        public UnityEngine.Events.UnityEvent OnExitActiveBound;
        public UnityEngine.Events.UnityEvent OnFinishChangingFollowPoint;
        public UnityEngine.Events.UnityEvent OnFinishChangingOrthoSize;


        public DataSettings dataSettings;

        private int m_Count = 0;
        private Cinemachine.CinemachineConfiner m_CinemachineConfiner;
        private Cinemachine.CinemachineVirtualCamera m_CinemachineVirtualCamera;
        private Transform m_CinemachineTransform;
        private Cinemachine.CinemachineFramingTransposer m_CinemachineComposer;
        private Collider2D m_OriginalCameraBound;
        private Transform m_OriginalCameraFollowPoint;
        private float m_OriginalOrthographicSize;
        private float m_OriginalDeadZoneWidth;
        private float m_OriginalDeadZoneHeight;

        private Transform m_VirtualTransform;

        private Transform m_DesiredTransform;
        private float m_DesiredOrthoSize;

        private Coroutine m_CurrentFollowingPointChagingCoroutine;
        private Coroutine m_CurrentOrthoSizeChagingCoroutine;

        //Timer
        private float m_MaxFollowingPointChangingTimer;
        private float m_MaxOrthoSizeChaingTimer;


        private void Awake()
        {
            m_CinemachineConfiner = FindObjectOfType<Cinemachine.CinemachineConfiner>();
            m_CinemachineVirtualCamera = FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();
            m_CinemachineTransform = m_CinemachineVirtualCamera.transform;
            m_CinemachineComposer = m_CinemachineVirtualCamera.GetCinemachineComponent<Cinemachine.CinemachineFramingTransposer>();
            m_OriginalCameraFollowPoint = m_CinemachineVirtualCamera.Follow;
            m_OriginalOrthographicSize = m_CinemachineVirtualCamera.m_Lens.OrthographicSize;
            m_OriginalCameraBound = m_CinemachineConfiner.m_BoundingShape2D;

            if (m_CinemachineComposer != null)
            {
                m_OriginalDeadZoneWidth = m_CinemachineComposer.m_DeadZoneWidth;
                m_OriginalDeadZoneHeight = m_CinemachineComposer.m_DeadZoneHeight;
            }

            m_VirtualTransform = new GameObject("VirtualTransform").transform;

            m_VirtualTransform.parent = gameObject.transform;



            GetComponent<Collider2D>().isTrigger = true;

            if (cameraBound != null)
            {
                cameraBound.isTrigger = true;
            }

        }

        public void ChangeToNewState(float delayTime = 0f)
        {
            StartCoroutine(InternalChangeToNewState(delayTime));
        }

        public void ChangeBackToOrginalState(float delayTime = 0f)
        {
            StartCoroutine(InternalChangeBackToOrginalState(delayTime));
        }

        private IEnumerator InternalChangeToNewState(float delayTime)
        {
            yield return new WaitForSeconds(delayTime);

            if (cameraBound != null)
            {                
                m_CinemachineConfiner.m_BoundingShape2D = cameraBound;
            }

            if (changeCameraFollowPoint)
            {
                m_VirtualTransform.position = m_CinemachineTransform.position;
                m_CinemachineVirtualCamera.Follow = m_VirtualTransform;

                m_DesiredTransform = cameraFollowPoint;

                if (m_CinemachineComposer != null)
                {

                    m_CinemachineComposer.m_DeadZoneWidth = 0;
                    m_CinemachineComposer.m_DeadZoneHeight = 0;

                }

                if (m_CurrentFollowingPointChagingCoroutine != null)
                    StopCoroutine(m_CurrentFollowingPointChagingCoroutine);
                m_CurrentFollowingPointChagingCoroutine = StartCoroutine(ChangeCameraFollowPoint());

            }

            if (changeOrthographicSize)
            {
                m_DesiredOrthoSize = orthographicSize;

                if (m_CurrentOrthoSizeChagingCoroutine != null)
                {
                    StopCoroutine(m_CurrentOrthoSizeChagingCoroutine);
                }

                m_CurrentOrthoSizeChagingCoroutine = StartCoroutine(ChangeOrthoSize());
            }

        }

        private IEnumerator InternalChangeBackToOrginalState(float delayTime)
        {
            yield return new WaitForSeconds(delayTime);

            if (cameraBound != null)
            {
                m_CinemachineConfiner.m_BoundingShape2D = m_OriginalCameraBound;
            }

            if (changeCameraFollowPoint)
            {
                m_VirtualTransform.position = m_CinemachineTransform.position;
                m_CinemachineVirtualCamera.Follow = m_VirtualTransform;
                m_DesiredTransform = m_OriginalCameraFollowPoint;


                if (m_CinemachineComposer != null)
                {
                    m_CinemachineComposer.m_DeadZoneWidth = 0;
                    m_CinemachineComposer.m_DeadZoneHeight = 0;

                }

                if (m_CurrentFollowingPointChagingCoroutine != null)
                    StopCoroutine(m_CurrentFollowingPointChagingCoroutine);

                m_CurrentFollowingPointChagingCoroutine = StartCoroutine(ChangeCameraFollowPoint());
            }

            if (changeOrthographicSize)
            {
                m_DesiredOrthoSize = m_OriginalOrthographicSize;

                if (m_CurrentOrthoSizeChagingCoroutine != null)
                {
                    StopCoroutine(m_CurrentOrthoSizeChagingCoroutine);
                }

                m_CurrentOrthoSizeChagingCoroutine = StartCoroutine(ChangeOrthoSize());
            }


        }


        private IEnumerator ChangeCameraFollowPoint()
        {
            m_MaxFollowingPointChangingTimer = (m_VirtualTransform.position - m_DesiredTransform.position).sqrMagnitude / (10f * followPointChangingSmoothSpeed * 10f * followPointChangingSmoothSpeed);


            //while((m_VirtualTransform.position - m_DesiredTransform.position).sqrMagnitude > 0.0001f)
            while ((!Mathf.Approximately(m_CinemachineTransform.position.x, m_DesiredTransform.position.x) || !Mathf.Approximately(m_CinemachineTransform.position.y, m_DesiredTransform.position.y)) && m_MaxFollowingPointChangingTimer > 0)
            {
                m_VirtualTransform.position = Vector3.MoveTowards(m_VirtualTransform.position, m_DesiredTransform.position, Time.deltaTime * 10f * followPointChangingSmoothSpeed);
                m_MaxFollowingPointChangingTimer -= Time.deltaTime;
                yield return null;
            }


            m_CinemachineVirtualCamera.Follow = m_DesiredTransform;
            m_CinemachineComposer.m_DeadZoneWidth = m_OriginalDeadZoneWidth;
            m_CinemachineComposer.m_DeadZoneHeight = m_OriginalDeadZoneHeight;

            OnFinishChangingFollowPoint.Invoke();
            m_MaxOrthoSizeChaingTimer = 0;
        }

        private IEnumerator ChangeOrthoSize()
        {
            m_MaxOrthoSizeChaingTimer = (m_CinemachineVirtualCamera.m_Lens.OrthographicSize - m_DesiredOrthoSize) / orthoSizeChangingSmoothSpeed;

            while (!Mathf.Approximately(m_CinemachineVirtualCamera.m_Lens.OrthographicSize, m_DesiredOrthoSize))
            {
                m_CinemachineVirtualCamera.m_Lens.OrthographicSize = Mathf.MoveTowards(m_CinemachineVirtualCamera.m_Lens.OrthographicSize, m_DesiredOrthoSize, orthoSizeChangingSmoothSpeed * Time.deltaTime);
                m_MaxOrthoSizeChaingTimer -= Time.deltaTime;
                yield return null;
            }

            m_CinemachineVirtualCamera.m_Lens.OrthographicSize = m_DesiredOrthoSize;

            OnFinishChangingOrthoSize.Invoke();
            m_MaxOrthoSizeChaingTimer = 0;
        }

        private IEnumerator DisableSchedule()
        {
            //Wait for seconds before disable because if we disable this gameobject to soon, the coroutines will be interrupted
            yield return new WaitForSeconds(m_MaxFollowingPointChangingTimer + m_MaxOrthoSizeChaingTimer + 5f);


            gameObject.SetActive(false);
            PersistentDataManager.SetDirty(this);

        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (disableBoundOnExit && m_Count == 1) return;

            if (targetLayers.Contains(collision.gameObject))
            {
                for (int i = 0; i < needToBeActiveGameObjects.Length; i++)
                {
                    needToBeActiveGameObjects[i].SetActive(true);
                }

                ChangeToNewState();

                OnEnterActiveBound.Invoke();

            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (disableBoundOnExit && m_Count == 1) return;

            if (targetLayers.Contains(collision.gameObject))
            {
                for (int i = 0; i < needToBeActiveGameObjects.Length; i++)
                {
                    needToBeActiveGameObjects[i].SetActive(false);
                }

                ChangeBackToOrginalState();

                OnExitActiveBound.Invoke();

                m_Count++;

                if (disableBoundOnExit)
                {
                    StartCoroutine(DisableSchedule());
                }
            }
        }

        void OnEnable()
        {
            PersistentDataManager.RegisterPersister(this);
        }

        void OnDisable()
        {
            PersistentDataManager.UnregisterPersister(this);
        }

        public DataSettings GetDataSettings()
        {
            return dataSettings;
        }

        public void LoadData(Data data)
        {
            Data<bool> savedData = (Data<bool>)data;
            gameObject.SetActive(savedData.value);
        }

        public Data SaveData()
        {
            return new Data<bool>(gameObject.activeSelf);
        }

        public void SetDataSettings(string dataTag, DataSettings.PersistenceType persistenceType)
        {
            dataSettings.dataTag = dataTag;
            dataSettings.persistenceType = persistenceType;
        }

    }
}
