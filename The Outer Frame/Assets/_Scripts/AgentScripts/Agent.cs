using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    [SerializeField] StateEnum SO;
    
    bool isActive = true;

    public void SetActiveOrDesactive(bool x)
    {
        isActive = x;
    }

    public bool GetIfIsActive()
    {
        return isActive;
    }
}
