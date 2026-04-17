using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockingRaycasterForCanvas : MonoBehaviour
{
    Canvas canvas;
    [SerializeField] LayerMask blockingLayer; 
    private GraphicRaycaster raycaster;
    bool isCanvasBlocking;
    RectTransform rectTransform;
    Camera cam;

    void Start()
    {
        canvas = GetComponent<Canvas>();
        raycaster = GetComponent<GraphicRaycaster>();
        rectTransform = canvas.GetComponent<RectTransform>();
        cam = Camera.main;
    }

    void Update()
    {
        CheckBlockingAtMouse(null);

        raycaster.enabled = !isCanvasBlocking;
    }

    void CheckBlockingAtMouse(BoxCollider colliderToIgnore = null)
    {
        if (!cam)
        {
            isCanvasBlocking = false;
            return;
        }

        Vector2 mousePos = Input.mousePosition;

        // Si el mouse no está sobre el canvas, no bloquear
        if (!RectTransformUtility.RectangleContainsScreenPoint(
            rectTransform,
            mousePos,
            cam))
        {
            isCanvasBlocking = false;
            return;
        }

        Vector3 worldPoint;

        if (!RectTransformUtility.ScreenPointToWorldPointInRectangle(
            rectTransform,
            mousePos,
            cam,
            out worldPoint))
        {
            isCanvasBlocking = false;
            return;
        }

        Vector3 origin = cam.transform.position;
        Vector3 dir = (worldPoint - origin).normalized;
        float distance = Vector3.Distance(origin, worldPoint);

        RaycastHit[] hits = Physics.RaycastAll(origin, dir, distance, blockingLayer);

        isCanvasBlocking = false;

        foreach (RaycastHit hit in hits)
        {
            if (colliderToIgnore != null && hit.collider == colliderToIgnore)
                continue;

            isCanvasBlocking = true;
            return;
        }
    }

    public bool GetIsCanvasBlocking(BoxCollider colliderToIgnore = null)
    {
        CheckBlockingAtMouse(colliderToIgnore);
        return isCanvasBlocking;
    }
}
