using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonElement : MonoBehaviour
{
    [SerializeField] int NumCam;
    [SerializeField] CamController.ViewStates View = CamController.ViewStates.SpecificView;
    private void Awake()
    {
        CamController.OnViewStateChanged += DisableBtnInSpecificWiew;
    }

    private void OnDestroy()
    {
        CamController.OnViewStateChanged -= DisableBtnInSpecificWiew;
    }

    private void OnMouseUpAsButton()
    {
        CamController.camController.UpdateCurrentView(NumCam);
        CamController.camController.UpdateViewState(View);
    }

    void DisableBtnInSpecificWiew(CamController.ViewStates view)
    {
        if (view == CamController.ViewStates.SpecificView)
        {
            GetComponent<BoxCollider>().enabled = false;
        }
        else if (view == CamController.ViewStates.GeneralView)
        {
            GetComponent<BoxCollider>().enabled = true;
        }
        else if (view == CamController.ViewStates.NotebookView)
        {
            GetComponent<BoxCollider>().enabled = false;
        }
    }
}

