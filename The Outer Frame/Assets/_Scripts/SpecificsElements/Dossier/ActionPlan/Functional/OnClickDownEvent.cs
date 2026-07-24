using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OnClickDownEvent : MonoBehaviour, IPointerDownHandler
{
    [System.Serializable]
    public class ButtonDownEvent : UnityEvent { }

    public ButtonDownEvent onPointerDown = new ButtonDownEvent();


    Button button;

    void Awake()
    {
        button = GetComponent<Button>();
    }

    bool IsInteractable()
    {
        return button == null || (button.enabled && button.interactable);
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        if (!IsInteractable()) return;

        onPointerDown.Invoke();
    }
}
