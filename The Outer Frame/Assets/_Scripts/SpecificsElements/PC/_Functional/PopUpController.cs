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
        //canvasRectTransform = _canvasRectTransform;
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
            Vector2 newPos = localPoint + (Vector2)offset;

            // Tamaño del popup
            Vector2 popupSize = rectTransform.sizeDelta;
            // Tamaño del canvas
            Vector2 canvasSize = canvasRectTransform.sizeDelta;

            // Calculamos límites considerando pivot en esquina superior izquierda
            float minX = 0;
            float maxX = canvasSize.x - popupSize.x + 2;
            float minY = -canvasSize.y + popupSize.y + 2;
            float maxY = 0;

            float clampedX = Mathf.Clamp(newPos.x, minX, maxX);
            float clampedY = Mathf.Clamp(newPos.y, minY, maxY);

            rectTransform.localPosition = new Vector2(clampedX, clampedY);
        }
    }
}
