using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MouseManager : MonoBehaviour
{
    private Vector3 worldPosition;
    [SerializeField] float XEdge;
    [SerializeField] float YEdge;

    float initialVerticalPos;
    float initialHorizontalPos;

    private void Start()
    {
        initialVerticalPos = transform.position.y;
        initialHorizontalPos = transform.position.x;
    }

    private void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane;

        worldPosition = Camera.main.ScreenToWorldPoint(mousePos);
        Debug.Log(worldPosition);

        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, mousePos.z);
        Vector3 offsetFromCenter = Camera.main.ScreenToWorldPoint(screenCenter) - worldPosition;

        float verticalOffset = offsetFromCenter.y * YEdge/100;
        float horizontalOffset = offsetFromCenter.x * XEdge/100;

        transform.position = new Vector3(transform.position.x, initialVerticalPos - verticalOffset, -(initialHorizontalPos - horizontalOffset));
    }
}
