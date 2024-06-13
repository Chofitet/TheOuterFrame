using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CamController : MonoBehaviour
{
    [SerializeField] Transform[] posCamara;
    [SerializeField] Transform CamTransform;
    [SerializeField] SmoothMoveObjectToPoints NotebookMove;
    public float transitionSpeed;
    Transform currentview { get; set; }
    int currentviewNum;
    ViewStates currentViewState;
    public static event Action<ViewStates> OnViewStateChanged;

    public static CamController camController { get; private set; }
    private void Awake()
    {

        if (camController != null && camController != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            camController = this;
        }

    }

    void Start()
    {
        currentview = CamTransform;
        currentviewNum = 0;
    }


    void Update()
    {
        currentview = posCamara[currentviewNum];

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            currentviewNum = 0;
            UpdateViewState(ViewStates.GeneralView);
        }

    }

    private void LateUpdate()
    {
        Quaternion currentAngle;

        CamTransform.position = Vector3.Lerp(CamTransform.position, currentview.position, Time.deltaTime * transitionSpeed);


        currentAngle = Quaternion.Lerp(CamTransform.rotation, currentview.rotation, Time.deltaTime * transitionSpeed);


        CamTransform.rotation = currentAngle;


    }

    public void UpdateCurrentView(int num)
    {
        currentviewNum = num;
        Debug.Log("Num of Cam: " + num);
    }

    public enum ViewStates
    {
        GeneralView,
        SpecificView,
        NotebookView
    }

    public void UpdateViewState(ViewStates newView)
    {
        switch (newView)
        {
            case ViewStates.GeneralView:
                break;
            case ViewStates.SpecificView:
                break;
            case ViewStates.NotebookView:
                NotebookMove.ChangePosition();
                break;

        }
        OnViewStateChanged?.Invoke(newView);
        currentViewState = newView;
    }

    

    public ViewStates GiveCurrentViewState()
    {
        return currentViewState;
    }

}
