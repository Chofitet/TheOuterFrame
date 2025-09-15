using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockingRaycasterForCanvas : MonoBehaviour
{
    Canvas canvas;
    [SerializeField] GameEvent gameEvent;
    [SerializeField] LayerMask blockingLayer; 
    private GraphicRaycaster raycaster;

    void Start()
    {
        canvas = GetComponent<Canvas>();
        raycaster = GetComponent<GraphicRaycaster>();
    }

    void Update()
    {
        // Obtener la posici�n del Canvas en el espacio world
        Vector3 canvasPosition = canvas.transform.position;

        // Crear un Ray desde la c�mara hacia el Canvas
        Ray ray = Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(canvasPosition));
        RaycastHit hit;

        // Si el Ray impacta con un objeto con BoxCollider entre la c�mara y el Canvas, desactivar interacci�n
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, blockingLayer))
        {
            // Si hay un objeto delante, deshabilitar el raycaster
            raycaster.enabled = false;
        }
        else
        {
            // Si no hay nada, permitir la interacci�n con el Canvas
            raycaster.enabled = true;
            
        }

    }
    

}
