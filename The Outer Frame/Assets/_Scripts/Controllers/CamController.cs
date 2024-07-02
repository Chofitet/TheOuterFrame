using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CamController : MonoBehaviour
{
    [SerializeField] Transform[] posCamara;
    [SerializeField] Transform CamTransform;
    [SerializeField] float transitionSpeed;
    Transform currentview { get; set; }
    int currentviewNum;

    void Start()
    {
        currentview = CamTransform;
        currentviewNum = 0;
    }

    private void Update()
    {
        currentview = posCamara[currentviewNum];
    }

    private void LateUpdate()
    {
        Quaternion currentAngle;

        CamTransform.position = Vector3.Lerp(CamTransform.position, currentview.position, Time.deltaTime * transitionSpeed);


        currentAngle = Quaternion.Lerp(CamTransform.rotation, currentview.rotation, Time.deltaTime * transitionSpeed);


        CamTransform.rotation = currentAngle;


    }

    public void UpdateCurrentView(Component sender, object View)
    {
        ViewStates newview = (ViewStates)View;
        switch(newview)
        {
            case ViewStates.GeneralView:
                currentviewNum = 0;
                break;
            case ViewStates.PinchofonoView:
                currentviewNum = 1;
                break;
            case ViewStates.BoardView:
                currentviewNum = 2;
                break;
            case ViewStates.PCView:
                currentviewNum = 3;
                break;
            case ViewStates.ProgressorView:
                currentviewNum = 4;
                break;
            case ViewStates.TVView:
                currentviewNum = 5;
                break;
        }
    }

}
