using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeScreenShoot : MonoBehaviour
{
    [SerializeField] bool BurstMode;
    [SerializeField] float BurstDuration;
    [SerializeField] float BurstFrecuency;
    
    int NumOfScreenshot = 0;
#if UNITY_EDITOR
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.P))
        {

            if(BurstMode) 
            {
                StartCoroutine(TakeBurstScreenshots(BurstDuration, BurstFrecuency));
            }

            Debug.Log("press P");
                // Resolución actual de la ventana de juego
                int width = Screen.width;
                int height = Screen.height;

                string fileName = $"Screenshot_{width}x{height}_{NumOfScreenshot}.png";
                ScreenCapture.CaptureScreenshot(fileName);

                Debug.Log($"Screenshot guardado: {fileName}");
                NumOfScreenshot += 1;
        }

        IEnumerator TakeBurstScreenshots(float duration, float interval)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                int width = Screen.width;
                int height = Screen.height;

                string fileName = $"Screenshot_{width}x{height}_{NumOfScreenshot}.png";
                ScreenCapture.CaptureScreenshot(fileName);

                Debug.Log($"Screenshot guardado: {fileName}");

                NumOfScreenshot++;

                yield return new WaitForSeconds(interval);
                elapsed += interval;
            }
        }
    }
#endif
}
