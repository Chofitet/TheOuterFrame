using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPCameraController : MonoBehaviour
{
    [SerializeField] private Transform cameraTarget;

    [Header("Look Settings")]
    [SerializeField] private float sensitivity = 2f;
    [SerializeField] private float verticalLimit = 89f;

    private float pitch;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        cameraTarget.Rotate(Vector3.up * mouseX, Space.World);

        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -verticalLimit, verticalLimit);

        Vector3 rotation = cameraTarget.localEulerAngles;
        rotation.x = pitch;

        cameraTarget.localEulerAngles = rotation;
    }
}
