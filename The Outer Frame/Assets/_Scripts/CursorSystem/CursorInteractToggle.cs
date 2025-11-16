using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CursorInteractToggle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    Toggle toggle;
    bool inside = false;

    void Awake()
    {
        toggle = GetComponent<Toggle>();
    }

    bool IsValid()
    {
        return gameObject.activeInHierarchy &&
               toggle != null &&
               toggle.enabled &&
               toggle.interactable;
    }

    void Update()
    {
        if (inside && !IsValid())
        {
            inside = false;
            CursorManager.CM.ExitInteractive();
            CursorManager.CM.ForceDefault();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (IsValid())
        {
            inside = true;
            CursorManager.CM.EnterInteractive();
        }
        else
        {
            inside = false;
            CursorManager.CM.ForceDefault();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (inside)
        {
            inside = false;
            CursorManager.CM.ExitInteractive();
        }
        else
        {
            CursorManager.CM.ForceDefault();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (IsValid())
            CursorManager.CM.ClickInteractive();
    }
}
