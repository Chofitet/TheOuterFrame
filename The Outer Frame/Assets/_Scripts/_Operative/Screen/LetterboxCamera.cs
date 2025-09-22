using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterboxCamera : MonoBehaviour
{
    [SerializeField] private float targetAspectWidth = 16f;
    [SerializeField] private float targetAspectHeight = 9f;

    void Start()
    {
        Camera cam = GetComponent<Camera>();

        // Aspecto nativo (ejemplo: 16/9)
        float targetAspect = targetAspectWidth / targetAspectHeight;
        // Aspecto actual de la pantalla
        float windowAspect = (float)Screen.width / (float)Screen.height;

        // Calcula el factor de escala
        float scaleHeight = windowAspect / targetAspect;

        if (scaleHeight < 1.0f)
        {
            // Pantalla más "alta" que 16:9 ? franjas negras arriba/abajo
            Rect rect = cam.rect;

            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;

            cam.rect = rect;
        }
        else
        {
            // Pantalla más "ancha" que 16:9 ? ocupa todo el alto (no habrá barras verticales)
            Rect rect = cam.rect;

            rect.width = 1.0f;
            rect.height = 1.0f;
            rect.x = 0;
            rect.y = 0;

            cam.rect = rect;
        }
    }
}
