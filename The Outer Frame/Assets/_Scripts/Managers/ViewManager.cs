using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewManager : MonoBehaviour
{
    [SerializeField] ViewStates StartView;
    [SerializeField] float delayBetweenViews;
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
    [SerializeField] GameEvent OnTutorialView;
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
    bool isInTutorial = false;
    bool delayingView;
    bool IsStuckInView;
    ViewStates StuckView;

    private void Start()
    {
        if (!isInTutorial)
        {
            OnDisableInput?.Invoke(this, null);
            Invoke("SetStartView", 0.6f);
        }
        else
        {
            UpdateViewState(null, ViewStates.TutorialView);
        }
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

        if (delayingView) return;

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (!isInTutorial)
            {
                if (currentviewState != ViewStates.GeneralView) CheckForBackToGeneralView();
            }
            else
            {
                if (currentviewState != ViewStates.TutorialView) CheckForBackToTutorialView();
            }
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
                if (isInTutorial) CheckForBackToTutorialView();
                else BackToGeneralView(null, null);
                isInPause = false;
                OnBackToPause?.Invoke(this, null);
            }
        }
    }

    void CheckForBackToGeneralView()
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

    void CheckForBackToTutorialView()
    {
        BackToTutrialView(null, null);
    }

    public void BackToGeneralView(Component sender, object _view)
    {
        
        OnNotebookLeave?.Invoke(this, null);
        UpdateViewState(this, ViewStates.GeneralView);
    }

    public void BackToTutrialView(Component sender, object _view)
    {
        OnNotebookLeave?.Invoke(this, null);
        UpdateViewState(this, ViewStates.TutorialView);
    }

    public void StuckMove(Component sender, object _view)
    {
        if (currentviewState == ViewStates.PauseView) return;
        if (IsStuckInView) UpdateViewState(this, StuckView);
    }
    public void UpdateViewState(Component sender, object _view)
    {
        if (delayingView) return;
        ViewStates NewView = (ViewStates)_view;
        if (NewView == currentviewState) return;
        switch (NewView)
        {
            case ViewStates.GeneralView:
                OnGeneralView?.Invoke(this, false);
                if(currentviewState == ViewStates.PCView) TimeManager.timeManager.NormalizeTime();
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
                if (currentviewState == ViewStates.BoardView) TimeManager.timeManager.NormalizeTime();
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
            case ViewStates.TutorialView:
                OnTutorialView?.Invoke(this, null);
                TimeManager.timeManager.PauseTime();
                break;
        }
        OnViewStateChange?.Invoke(this, NewView);
        currentviewState = NewView;
        StartCoroutine(DelayBetweenViews());
        if(currentviewState != ViewStates.PauseView) if (isInPause) isInPause = false;
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

    public void SetIsInTutorial(Component sender, object obj)
    {
        isInTutorial = (bool)obj;
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

    IEnumerator DelayBetweenViews()
    {
        delayingView = true;
        yield return new WaitForSeconds(delayBetweenViews);
        delayingView = false;
    }

    public void SetStuck(Component sender, object obj)
    {
        IsStuckInView = true;
        StuckView = (ViewStates)obj;
    }

    public void UnsetStuck(Component sender, object obj)
    {
        IsStuckInView = false;
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
    DrawerView,
    TutorialView
}
