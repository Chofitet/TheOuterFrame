using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class VoidCameraController : MonoBehaviour
{
    [SerializeField] List<CinemachineVirtualCamera> CamerasPos = new List<CinemachineVirtualCamera>();
    [SerializeField] GameEvent OnChangeScene;
    [SerializeField] GameEvent OnBackInVoid;
    int currentClicks;
    [SerializeField]  float coldDownToClick;
    bool waitForColdDown = true;
    Coroutine ColdDownClickCoroutine;

    [SerializeField] List<GameEvent> GoAwaySoundsEvents = new List<GameEvent>();

    private void Start()
    {
        StartCoroutine(ColdDownClick());
    }

    void Update()
    {
        if (waitForColdDown) return;

        if (Input.GetMouseButtonDown(1)) 
        {
            GoAwaySoundsEvents[currentClicks]?.Invoke(this, null);
            if (currentClicks == 0) SetPriority(1);
            else if (currentClicks == 1) SetPriority(2);
            else if (currentClicks == 2) SetPriority(3);
            else if (currentClicks == 3)
            {
                SetPriority(4);
                OnChangeScene?.Invoke(this, "LoadingScreen");
            }

            if (ColdDownClickCoroutine != null) StopCoroutine(ColdDownClickCoroutine);

            ColdDownClickCoroutine = StartCoroutine(ColdDownClick());
            OnBackInVoid?.Invoke(this, null);
            currentClicks++;
        }
    }

    public void BackToTheFirstCam(Component sender, object obj)
    {
        SetPriority(0);
        if (ColdDownClickCoroutine != null) StopCoroutine(ColdDownClickCoroutine);
        waitForColdDown = false;
        currentClicks = 0;
    }

    void SetPriority(int num)
    {
        foreach (CinemachineVirtualCamera VRcam in CamerasPos) VRcam.Priority = 0;
        CamerasPos[num].Priority = 1000;
    }


    IEnumerator ColdDownClick()
    {
        waitForColdDown = true;
        yield return new WaitForSeconds(coldDownToClick);
        waitForColdDown = false;
    }

}
