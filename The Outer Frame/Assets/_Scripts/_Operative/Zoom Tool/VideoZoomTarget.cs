using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoZoomTarget : MonoBehaviour
{
    [SerializeField] float ZoomValue;
    [SerializeField] float OffsetPosition;
    [SerializeField] Axis OffsetAxis;
    [SerializeField] CinemachineVirtualCamera Cam;

    private void Start()
    {
        PositionCam();
    }

    public enum Axis
    {
        X,
        Y,
        Z
    }


#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            PositionCam();
        }
    }
#endif


    void PositionCam()
    {
        if (Cam == null) return;

        Vector3 direction = GetAxisDirection();

        Vector3 targetPos = transform.position + direction * OffsetPosition;

        Cam.transform.position = targetPos;

        Cam.m_Lens.FieldOfView = ZoomValue;
    }

    Vector3 GetAxisDirection()
    {
        switch (OffsetAxis)
        {
            case Axis.X: return transform.right;
            case Axis.Y: return transform.up;
            case Axis.Z: return transform.forward;
            default: return Vector3.right;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            GoToZoomPosition(1001);
        }
        else if(Input.GetKeyUp(KeyCode.T))
        {
            GoToZoomPosition(0);
        }
    }

    public void GoToZoomPosition(int PRIORITY)
    {
        Cam.Priority = PRIORITY;
    }
}
