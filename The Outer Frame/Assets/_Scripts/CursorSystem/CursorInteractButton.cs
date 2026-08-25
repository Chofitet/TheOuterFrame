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

    bool isInsideAux;
    public void OnPointerEnter(PointerEventData eventData)
    {
        isInsideAux = true;
        
        if (!IsValid()) return;
        isInside = true;
        
        CursorManager.CM.EnterInteractive();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isInsideAux = false;
        if (!isInside) return;

        isInside = false;
        CursorManager.CM.ExitInteractive();
    }

    public void OnPointerDown(PointerEventData eventData)
    {

        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (!isInside) return; //despues este
        CursorManager.CM.ClickInteractive();
    }

    void Update()
    {
        bool valid = IsValid();

        // Se volvió interactuable mientras el mouse estaba encima
        if (!wasValid && valid && isInsideAux && !isAlwaysValid)
        {
            CursorManager.CM.EnterInteractive();
        }

        // Dejó de ser interactuable mientras el cursor estaba encima
        if (wasValid && !valid && isInsideAux && !isAlwaysValid)
        {
            isInsideAux = false;
            isInside = false;
            CursorManager.CM.ExitInteractive();

        }

        wasValid = valid;
    }

    private void OnDisable()
    {
        if (!isInside) return;

        //primero este
        Invoke("MarkInsideFalse", 0.01f);
        CursorManager.CM.ExitInteractive();
    }

    void MarkInsideFalse()
    {
        isInside = false;
    }

}
