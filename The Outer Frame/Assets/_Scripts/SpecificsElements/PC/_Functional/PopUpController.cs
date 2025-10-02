using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PopUpController : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    [SerializeField] TMP_Text text;
    [SerializeField] Button btn;
    RectTransform rectTransform;
    RectTransform canvasRectTransform;
    private Vector2 offset;
    [SerializeField] PopupView view;

    public void Initialize(string _text, RectTransform  _canvasRectTransform)
    {
        text.text = _text;
        canvasRectTransform = _canvasRectTransform;
        rectTransform = GetComponent<RectTransform>();
        view.init();
    }

    public Vector2 GetPopUpSize()
    {
        return GetComponent<RectTransform>().sizeDelta;
    }
    public Button GetButton()
    {
        return btn;
    }

    public void ClosePopUp()
    {
        Destroy(gameObject);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Convertimos la posición inicial del mouse a local en el rect
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out var localPoint);

        offset = rectTransform.localPosition - (Vector3)localPoint;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out var localPoint))
        {
            rectTransform.localPosition = localPoint + offset;
        }
    }
}
