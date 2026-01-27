using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CursorInteractButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    Button btn;
    bool inside = false;

    void Awake()
    {
        btn = GetComponent<Button>();
    }

    bool IsValid()
    {
        return gameObject.activeInHierarchy && btn != null && btn.enabled && btn.interactable;
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

    void OnDisable()
    {
        if (inside)
        {
            inside = false;
            CursorManager.CM.ExitInteractive();
            CursorManager.CM.ForceDefault();
        }
    }

}
