using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnjustBar : MonoBehaviour
{
    [SerializeField] RectTransform viewport;
    [SerializeField] Scrollbar verticalScrollbar;
    [SerializeField] float scrollbarWidth = 20f;

    float lerpSpeed = 10f;

    private ScrollRect scrollRect;
    private float targetOffsetX;
    private bool lastVisible;


    void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
        if (!viewport) viewport = scrollRect.viewport;
    }

    void Update()
    {
        bool visible = verticalScrollbar.gameObject.activeSelf;

        // cuando el estado de visibilidad cambia, seteamos el nuevo target
        if (visible != lastVisible)
        {
            targetOffsetX = visible ? -scrollbarWidth : 0f;
            lastVisible = visible;
        }

        // Lerp suave hacia el valor objetivo
        Vector2 offset = viewport.offsetMax;
        offset.x = Mathf.Lerp(offset.x, targetOffsetX, Time.deltaTime * lerpSpeed);
        viewport.offsetMax = offset;
    }
}
