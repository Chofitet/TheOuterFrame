using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CursorInteractButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (GetComponent<Button>().enabled == false) return;
        CursorManager.CM.SetInteractCursor();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (GetComponent<Button>().enabled == false) return;
        CursorManager.CM.SetDefaultCursor();
    }

}
