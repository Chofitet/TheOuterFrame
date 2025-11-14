using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CursorInteractButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    bool isStillOverInteractive;
    public void OnPointerEnter(PointerEventData eventData)
    {
        isStillOverInteractive = true;
        if (GetComponent<Button>().enabled == false) return;
        CursorManager.CM.SetInteractCursor();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isStillOverInteractive = false;
        if (GetComponent<Button>().enabled == false) return;
        CursorManager.CM.SetDefaultCursor();
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isStillOverInteractive)
        {
            Debug.Log("click");
            CursorManager.CM.SetInteractCursor();
        }
    }

}
