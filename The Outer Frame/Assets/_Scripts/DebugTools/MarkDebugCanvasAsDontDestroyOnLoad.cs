using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkDebugCanvasAsDontDestroyOnLoad : MonoBehaviour
{
    public static MarkDebugCanvasAsDontDestroyOnLoad Instance { get; private set; }


    void Start()
    {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
    }

}
