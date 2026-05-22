using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NoRememberWords : MonoBehaviour
{
    [SerializeField] GameEvent OnChangeScene;
    [SerializeField] int ClicksToChangeScene = 3;

    int currentClicks;

    void Update()
    {
        if (Input.GetMouseButtonDown(1)) // Click derecho
        {
            currentClicks++;

            if (currentClicks >= ClicksToChangeScene)
            {
                OnChangeScene?.Invoke(this, "LoadingScreen");
            }
        }
    }
}
