using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ActiveBound : MonoBehaviour {

    public GameObject target;
    public GameObject[] needActiveGameObjects;

    [Header("Camera Bound")]
    [Tooltip("if set, when the target enter this bound, the cinemachine confinder will be changed to this specified bound")]
    public PolygonCollider2D cameraBound;

    [Header("Camera Follow Point")]
    public bool changeCameraFollowPoint = true;
    [Tooltip("when the target enter the bound, the camera will follow this point")]
    public Transform cameraFollowPoint;

    [Header("Orthographic Size")]
    public bool changeOrthographicSize = false;
    public float orthographicSize = 3.5f;

    [Space(7)]
    public bool onlyOneTime = false;

    [Header("Events")]
    public UnityEngine.Events.UnityEvent OnEnterActiveBound;
    public UnityEngine.Events.UnityEvent OnExitActiveBound;


    private int m_Count = 0;
    private Cinemachine.CinemachineConfiner m_CinemachineConfiner;
    private Cinemachine.CinemachineVirtualCamera m_CinemachineVirtualCamera;
    private Collider2D m_OriginalCameraBound;
    private Transform m_OriginalCameraFollowPoint;
    private float m_OriginalOrthographicSize;



    private void Awake()
    {
        m_CinemachineConfiner = FindObjectOfType<Cinemachine.CinemachineConfiner>();
        m_CinemachineVirtualCamera = FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();

        GetComponent<Collider2D>().isTrigger = true;

        if(cameraBound!=null)
        {
            cameraBound.isTrigger = true;
        }

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (onlyOneTime && m_Count == 1) return;

        if(collision.gameObject == target)
        {
            for (int i = 0; i < needActiveGameObjects.Length; i++)
            {
                needActiveGameObjects[i].SetActive(true);
            }

            if(cameraBound!=null)
            {
                m_OriginalCameraBound = m_CinemachineConfiner.m_BoundingShape2D;
                m_CinemachineConfiner.m_BoundingShape2D = cameraBound;
            }

            if(changeCameraFollowPoint)
            {
                m_OriginalCameraFollowPoint = m_CinemachineVirtualCamera.Follow;
                m_CinemachineVirtualCamera.Follow = cameraFollowPoint;
            }

            if(changeOrthographicSize)
            {
                m_OriginalOrthographicSize = m_CinemachineVirtualCamera.m_Lens.OrthographicSize;
                m_CinemachineVirtualCamera.m_Lens.OrthographicSize = orthographicSize;
            }

            OnEnterActiveBound.Invoke();

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (onlyOneTime && m_Count==1) return;

        if (collision.gameObject == target)
        {
            for (int i = 0; i < needActiveGameObjects.Length; i++)
            {
                needActiveGameObjects[i].SetActive(false);
            }

            if (cameraBound != null)
            {
                m_CinemachineConfiner.m_BoundingShape2D = m_OriginalCameraBound;
            }

            if(changeCameraFollowPoint)
            {
                m_CinemachineVirtualCamera.Follow = m_OriginalCameraFollowPoint;
            }

            if (changeOrthographicSize)
            {
                m_CinemachineVirtualCamera.m_Lens.OrthographicSize = m_OriginalOrthographicSize;
            }

            OnExitActiveBound.Invoke();

            m_Count++;
        }
    }




}
