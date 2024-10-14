using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cinemachine;

public class CamController : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera[] posCamara;
    ViewStates actualView;

    public void UpdateCurrentView(Component sender, object View)
    {
        ViewStates newview = (ViewStates)View;
        actualView = newview;
        switch (newview)
        {
            case ViewStates.GeneralView:
                SetPriority(0);
                break;
            case ViewStates.DossierView:
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
            case ViewStates.OnTakenPaperView:
                SetPriority(0);
                break;
            case ViewStates.GameOverView:
                SetPriority(7);
                break;
            case ViewStates.PauseView:
                SetPriority(8);
                break;
            case ViewStates.DrawerView:
                SetPriority(9);
                break;
        }
    }

    public void AccelerateTime(Component sender, object obj)
    {
        bool isInspeedTime = (bool) obj;

        if (isInspeedTime) SetPriority(6);
        else if (actualView == ViewStates.GeneralView) SetPriority(0);

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
