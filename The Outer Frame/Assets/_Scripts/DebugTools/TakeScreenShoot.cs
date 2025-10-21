using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeScreenShoot : MonoBehaviour
{
    int NumOfScreenshot = 0;

    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("press P");
                // Resolución actual de la ventana de juego
                int width = Screen.width;
                int height = Screen.height;

                string fileName = $"Screenshot_{width}x{height}_{NumOfScreenshot}.png";
                ScreenCapture.CaptureScreenshot(fileName);

                Debug.Log($"Screenshot guardado: {fileName}");
                NumOfScreenshot += 1;
        }
    }
#endif
}
