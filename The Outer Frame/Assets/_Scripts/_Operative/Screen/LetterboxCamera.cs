using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterboxCamera : MonoBehaviour
{
    [SerializeField] private float targetAspectWidth = 16f;
    [SerializeField] private float targetAspectHeight = 9f;

    [SerializeField] private int forcedWidth = 1920;
    [SerializeField] private int forcedHeight = 1080;
    [SerializeField] private bool forceResolution = false;

    private int lastWidth;
    private int lastHeight;

    void Start()
    {
        SetRatio();

        /*if (forceResolution)
        {
            Screen.SetResolution(forcedWidth, forcedHeight, FullScreenMode.Windowed);
        }*/

       
    }

    void SetRatio()
    {
        Camera cam = GetComponent<Camera>();

        float targetAspect = targetAspectWidth / targetAspectHeight;
        float windowAspect = (float)Screen.width / Screen.height;

        float scaleHeight = windowAspect / targetAspect;

        if (scaleHeight < 1.0f)
        {
            // Barras arriba y abajo
            Rect rect = cam.rect;

            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;

            cam.rect = rect;
        }
        else
        {
            // Barras a izquierda y derecha
            float scaleWidth = 1.0f / scaleHeight;

            Rect rect = cam.rect;

            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0;

            cam.rect = rect;
        }
    }

    void Update()
    {
        if (Screen.width != lastWidth || Screen.height != lastHeight)
        {
            lastWidth = Screen.width;
            lastHeight = Screen.height;

            SetRatio();
        }
    }
}
