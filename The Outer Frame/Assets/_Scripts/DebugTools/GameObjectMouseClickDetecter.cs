using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameObjectMouseClickDetecter : MonoBehaviour
{
    private GraphicRaycaster graphicRaycaster;
    private EventSystem eventSystem;

    private void Start()
    {
        // Encuentra el GraphicRaycaster en el World Canvas
        graphicRaycaster = GetComponent<GraphicRaycaster>();
        eventSystem = FindObjectOfType<EventSystem>();

        if (graphicRaycaster == null || eventSystem == null)
        {
            Debug.LogError("Asegúrate de que el World Canvas tenga un GraphicRaycaster y que haya un EventSystem en la escena.");
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 0 es el botón izquierdo del mouse
        {
            DetectUICollisionsAtMousePosition();
        }
    }

    private void DetectUICollisionsAtMousePosition()
    {
        PointerEventData pointerData = new PointerEventData(eventSystem)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        graphicRaycaster.Raycast(pointerData, results);

        if (results.Count > 0)
        {
            List<string> uiObjectNames = new List<string>();
            foreach (RaycastResult result in results)
            {
                uiObjectNames.Add(result.gameObject.name);
            }
            Debug.Log("UI objects clicked on World Canvas: " + string.Join(", ", uiObjectNames));
        }
        else
        {
            Debug.Log("Clicked on empty UI space in World Canvas.");
        }
    }
}
