using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cinemachine;

public class CamController : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera[] posCamara;
    ViewStates actualView;
    float delayTime;
    float _delay = 0f;
    public void UpdateCurrentView(Component sender, object View)
    {
        ViewStates newview = (ViewStates)View;
        actualView = newview;
        switch (newview)
        {
            case ViewStates.GeneralView:
                SetPriority(0);
                delayTime = 0;
                break;
            case ViewStates.DossierView:
                SetPriority(0);
                delayTime = _delay;
                break;
            case ViewStates.PinchofonoView:
                SetPriority(1);
                delayTime = 0;
                break;
            case ViewStates.BoardView:
                SetPriority(2);
                delayTime = 0;
                break;
            case ViewStates.PCView:
                SetPriority(3);
                delayTime = 0;
                break;
            case ViewStates.ProgressorView:
                SetPriority(4);
                delayTime = 0;
                break;
            case ViewStates.TVView:
                SetPriority(5);
                break;
            case ViewStates.OnTakenPaperView:
                SetPriority(0);
                delayTime = _delay;
                break;
            case ViewStates.GameOverView:
                SetPriority(7);
                delayTime = 0;
                break;
            case ViewStates.PauseView:
                SetPriority(8);
                delayTime = 0;
                break;
            case ViewStates.DrawerView:
                SetPriority(9);
                delayTime = 0;
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
        StartCoroutine(DelayChangeView(num));
        
    }

    IEnumerator DelayChangeView(int num)
    {
        yield return new WaitForSeconds(delayTime);
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
