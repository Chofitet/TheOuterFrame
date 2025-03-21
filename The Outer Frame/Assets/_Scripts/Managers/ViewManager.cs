using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewManager : MonoBehaviour
{
    [SerializeField] ViewStates StartView;
    [SerializeField] float DelayBetweenViews;
    [SerializeField] GameEvent OnGeneralView;
    [SerializeField] GameEvent OnPinchofonoView;
    [SerializeField] GameEvent OnBoardView;
    [SerializeField] GameEvent OnPCWiew;
    [SerializeField] GameEvent OnProgressorView;
    [SerializeField] GameEvent OnTVView;
    [SerializeField] GameEvent OnDossierView;
    [SerializeField] GameEvent OnViewStateChange;
    [SerializeField] GameEvent OnNotebookTake;
    [SerializeField] GameEvent OnNotebookLeave;
    [SerializeField] GameEvent OnTakeSomeInBoard;
    [SerializeField] GameEvent OnTakenPaperView;
    [SerializeField] GameEvent OnGameOverView;
    [SerializeField] GameEvent OnPauseView;
    [SerializeField] GameEvent OnDrawerView;
    [SerializeField] GameEvent OnBackToPause;
    [SerializeField] GameEvent OnSitDownSound;
    [SerializeField] GameEvent OnSendReportAutomatically;
    [SerializeField] GameEvent OnEnableInput;
    [SerializeField] GameEvent OnDisableInput;
    bool isAPaperHolding;
    ViewStates currentviewState;
    bool isInputDisable;
    bool isInPause;
    bool isGameOver;
    bool inOnFinalReport;

    private void Start()
    {
        OnDisableInput?.Invoke(this, null);
        Invoke("SetStartView", 0.6f);
    }

    void SetStartView()
    {
        OnEnableInput?.Invoke(this, null);
        UpdateViewState(null, StartView);
    }

    void Update()
    {
        if (isInputDisable)
        {
            Debug.Log("disable");
            return;
        }

        if (Input.GetKeyDown(KeyCode.Mouse1) && currentviewState != ViewStates.GeneralView)
        {
            if (inOnFinalReport) OnSendReportAutomatically?.Invoke(this, null);

            if (currentviewState == ViewStates.OnTakeSomeInBoard)
            {
                UpdateViewState(this, ViewStates.BoardView);
                return;
            }
            if (isAPaperHolding)
            {
                UpdateViewState(this, ViewStates.OnTakenPaperView);
                return;
            }
            if (currentviewState == ViewStates.BoardView) TimeManager.timeManager.NormalizeTime();

            if (isInPause)
            {
                TimeManager.timeManager.NormalizeTime();
                OnBackToPause?.Invoke(this, null);
                isInPause = false;
            }

            BackToGeneralView(null, null);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isInPause)
            {
                TimeManager.timeManager.PauseTime();
                UpdateViewState(null, ViewStates.PauseView);

                isInPause = true;
            }
            else
            {
                TimeManager.timeManager.NormalizeTime();
                BackToGeneralView(null, null);
                isInPause = false;
                OnBackToPause?.Invoke(this, null);
            }
        }
    }

    public void BackToGeneralView(Component sender, object _view)
    {
        
        OnNotebookLeave?.Invoke(this, null);
        UpdateViewState(this, ViewStates.GeneralView);
    }

    public void UpdateViewState(Component sender, object _view)
    {
        ViewStates NewView = (ViewStates)_view;
        if (NewView == currentviewState) return;
        switch (NewView)
        {
            case ViewStates.GeneralView:
                OnGeneralView?.Invoke(this, false);
                TimeManager.timeManager.NormalizeTime();
                BackToGeneralViewWhitMoving();
                break;
            case ViewStates.PinchofonoView:
                if (isGameOver) return;
                if (inOnFinalReport) { OnSendReportAutomatically?.Invoke(this, null); return; }
                OnNotebookTake.Invoke(this, true);
                OnPinchofonoView?.Invoke(this, null);
                break;
            case ViewStates.BoardView:
                if (isGameOver) return;
                if (inOnFinalReport) { OnSendReportAutomatically?.Invoke(this, null); return; }
                TimeManager.timeManager.PauseTime();
                OnBoardView?.Invoke(this, null);
                OnNotebookTake.Invoke(this, true);
                break;
            case ViewStates.PCView:
                if (isGameOver) return;
                TimeManager.timeManager.SetAnotherSpeed(0.5f);
                OnPCWiew?.Invoke(this, null);
                OnNotebookTake.Invoke(this, true);
                break;
            case ViewStates.ProgressorView:
                if (isGameOver) return;
                if (inOnFinalReport) { OnSendReportAutomatically?.Invoke(this, null);}
                OnProgressorView?.Invoke(this, null);
                break;
            case ViewStates.TVView:
                if (inOnFinalReport) { OnSendReportAutomatically?.Invoke(this, null); return; }
                TimeManager.timeManager.SetAnotherSpeed(0.75f);
               OnTVView?.Invoke(this, null);
                OnNotebookTake.Invoke(this, true);
                break;
            case ViewStates.DossierView:
                if (isGameOver) return;
                OnDossierView?.Invoke(this, null);
                OnNotebookTake.Invoke(this, true);
                break;
            case ViewStates.OnTakenPaperView:
                if (isGameOver) return;
                OnTakenPaperView?.Invoke(this, null);
                OnNotebookTake.Invoke(this, true);
                break;
            case ViewStates.OnTakeSomeInBoard:
                if (isGameOver) return;
                if (inOnFinalReport) { OnSendReportAutomatically?.Invoke(this, null); return; }
                OnTakeSomeInBoard?.Invoke(this, null);
                break;
            case ViewStates.GameOverView:
                if (inOnFinalReport) return;
                TimeManager.timeManager.NormalizeTime();
                OnGameOverView.Invoke(this, "RetryMenu");
                OnNotebookTake.Invoke(this, false);
                isGameOver = true;
                break;
            case ViewStates.PauseView:
                if (isGameOver) return;
                OnPauseView?.Invoke(this, null);
                break;
            case ViewStates.DrawerView:
                OnDrawerView?.Invoke(this, null);
                OnNotebookTake.Invoke(this, false);
                break;
        }
        OnViewStateChange?.Invoke(this, NewView);
        currentviewState = NewView;

        //Debug.Log(currentviewState);
    }

    public ViewStates GiveCurrentViewState()
    {
        return currentviewState;
    }

    public void OnSetPaperState(Component sender, object obj)
    {
        bool x = (bool)obj;

        isAPaperHolding = x;
    }

    void BackToGeneralViewWhitMoving()
    {
        if (currentviewState != ViewStates.DossierView && currentviewState != ViewStates.OnTakenPaperView )
        {
            OnSitDownSound?.Invoke(this, null);
        }
    }

    public void EnableInput(Component sender, object _view)
    {
        if(isInputDisable)
        {
            isInputDisable = false;
        }
    }

    public void DisableInput(Component sender,object obj)
    {
        if (!isInputDisable)
        {
            isInputDisable = true;
        }
    }

    public void OnFinalReportTake(Component sender, object obj)
    {
        inOnFinalReport = true;
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
    OnTakenPaperView,
    OnTakeSomeInBoard,
    GameOverView,
    PauseView,
    DrawerView
}
