using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotebookProcessManager : MonoBehaviour
{
    private int activeProcesses = 0;
    public event Action OnAllProcessesFinished;
    public event Action OnProcessStarted;

    public void RegisterProcess()
    {
        activeProcesses++;
        OnProcessStarted.Invoke();
    }

    public void UnregisterProcess()
    {
        activeProcesses--;
        if (activeProcesses <= 0)
        {
            Invoke("invokeOnAllProcessesFinished", 0.7f);
            activeProcesses = 0;
        }
    }

    void invokeOnAllProcessesFinished()
    {
        OnAllProcessesFinished?.Invoke();
        Debug.Log("All Notebook process end");
    }

    public bool IsProcessing => activeProcesses > 0;
}
