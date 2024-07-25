using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewManager : MonoBehaviour
{
    [SerializeField] GameEvent OnGeneralView;
    [SerializeField] GameEvent OnPinchofonoView;
    [SerializeField] GameEvent OnBoardView;
    [SerializeField] GameEvent OnPCWiew;
    [SerializeField] GameEvent OnProgressorView;
    [SerializeField] GameEvent OnTVView;
    [SerializeField] GameEvent OnDossierView;
    [SerializeField] GameEvent OnPrinterView;
    [SerializeField] GameEvent OnViewStateChange;
    [SerializeField] GameEvent OnNotebookTake;
    [SerializeField] GameEvent OnNotebookLeave;
    [SerializeField] GameEvent OnCallTranscriptionView;
    ViewStates currentviewState;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1) && currentviewState != ViewStates.GeneralView)
        {
            BackToGeneralView(null, null);
        }
    }

    public void BackToGeneralView(Component sender, object _view)
    {
        TimeManager.timeManager.NormalizeTime();
        OnNotebookLeave?.Invoke(this, null);
        UpdateViewState(this, ViewStates.GeneralView);
    }

    public void UpdateViewState(Component sender, object _view)
    {
        ViewStates NewView = (ViewStates)_view;
        switch (NewView)
        {
            case ViewStates.GeneralView:
                OnGeneralView?.Invoke(this, false);
                break;
            case ViewStates.PinchofonoView:
                OnNotebookTake.Invoke(this, true);
                OnPinchofonoView?.Invoke(this, null);
                break;
            case ViewStates.BoardView:
                TimeManager.timeManager.PauseTime();
                OnBoardView?.Invoke(this, null);
                OnNotebookTake.Invoke(this, true);
                break;
            case ViewStates.PCView:
                OnPCWiew?.Invoke(this, null);
                OnNotebookTake.Invoke(this, true);
                break;
            case ViewStates.ProgressorView:
                OnProgressorView?.Invoke(this, null);
                break;
            case ViewStates.TVView:
                OnTVView?.Invoke(this, null);
                break;
            case ViewStates.DossierView:
                OnDossierView?.Invoke(this, null);
                OnNotebookTake.Invoke(this, true);
                break;
            case ViewStates.PrinterView:
                OnPrinterView?.Invoke(this, null);
                break;
            case ViewStates.OnCallTranscriptionView:
                OnCallTranscriptionView?.Invoke(this, null);
                OnNotebookLeave?.Invoke(this, null);
                break;


        }
        OnViewStateChange?.Invoke(this,NewView);
        
        currentviewState = NewView;
        Debug.Log(currentviewState);
    }

    public ViewStates GiveCurrentViewState()
    {
        return currentviewState;
    }
}

public enum ViewStates
{
    GeneralView,
    PinchofonoView,
    BoardView,
    PCView,
    ProgressorView,
    TVView,
    DossierView,
    PrinterView,
    OnCallTranscriptionView,
}
