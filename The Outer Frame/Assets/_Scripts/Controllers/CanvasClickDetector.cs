using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CanvasClickDetector : MonoBehaviour
{
    [SerializeField] GameEvent GameEvent;
    GraphicRaycaster raycaster;
    BlockingRaycasterForCanvas BlockingRaycast;

    void Awake()
    {
        raycaster = GetComponent<GraphicRaycaster>();
        BlockingRaycast = GetComponent<BlockingRaycasterForCanvas>();
    }

    void Update()
    {
        //Check if the left Mouse button is clicked
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            List<RaycastResult> results = new List<RaycastResult>();

            pointerData.position = Input.mousePosition;
            raycaster.Raycast(pointerData, results);

            foreach (RaycastResult result in results)
            {
                if(raycaster.enabled) GameEvent?.Invoke(this, null);
            }
        }
    }
}
