using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CursorInteractButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    private Button btn;

    private bool isInside;
    private bool wasValid;
    [SerializeField] bool isAlwaysValid;

    void Awake()
    {
        btn = GetComponent<Button>();
        wasValid = IsValid();
    }

    bool IsValid()
    {
        return gameObject.activeInHierarchy && btn != null &&btn.enabled && btn.interactable;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!IsValid()) return;

        isInside = true;
        CursorManager.CM.EnterInteractive();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isInside) return;

        isInside = false;
        CursorManager.CM.ExitInteractive();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isInside) return;
        CursorManager.CM.ClickInteractive();
    }

    void Update()
    {
        bool valid = IsValid();

        // Si el botón dejó de ser interactuable mientras el cursor estaba encima,
        // simulamos un PointerExit.
        if (wasValid && !valid && isInside && !isAlwaysValid)
        {
            isInside = false;
            CursorManager.CM.ExitInteractive();
        }

        wasValid = valid;
    }

    private void OnDisable()
    {
        if (!isInside) return;

        isInside = false;
        CursorManager.CM.ExitInteractive();
    }

}
