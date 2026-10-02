using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private float moveSpeed = 3f;

    private CharacterController controller;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void FixedUpdate()
    {
        float movementDirection = 0f;

        if (Input.GetMouseButton(0))
            movementDirection += 1f;

        if (Input.GetMouseButton(1))
            movementDirection -= 1f;

        if (movementDirection == 0)
            return;

        float yaw = cameraTarget.eulerAngles.y;

        Vector3 forward = Quaternion.Euler(0f, yaw, 0f) * Vector3.forward;

        Vector3 movement = forward * movementDirection * moveSpeed;

        Debug.Log(movement);

        controller.Move(movement * Time.deltaTime);
    }
}
