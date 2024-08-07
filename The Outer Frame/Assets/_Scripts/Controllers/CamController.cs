using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cinemachine;

public class CamController : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera[] posCamara;

    

    public void UpdateCurrentView(Component sender, object View)
    {
        ViewStates newview = (ViewStates)View;
        switch(newview)
        {
            case ViewStates.GeneralView:
                SetPriority(0);
                break;
            case ViewStates.PinchofonoView:
                SetPriority(1);
                break;
            case ViewStates.BoardView:
                SetPriority(2);
                break;
            case ViewStates.PCView:
                SetPriority(3);
                break;
            case ViewStates.ProgressorView:
                SetPriority(4);
                break;
            case ViewStates.TVView:
                SetPriority(5);
                break;
            case ViewStates.OnCallTranscriptionView:
                SetPriority(0);
                break;
        }
    }

    void SetPriority(int num)
    {
        posCamara[num].Priority = 100;
        SetOthersPriorityZero(posCamara[num]);
    }

    void SetOthersPriorityZero(CinemachineVirtualCamera exception)
    {
        foreach(CinemachineVirtualCamera cam in posCamara)
        {
            if (cam != exception) cam.Priority = 0;
        }
    }

}
