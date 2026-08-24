using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    [SerializeField] GameEvent OnZoomView;
    [SerializeField] GameEvent OnBackToPause;
    [SerializeField] GameEvent OnSitDownSound;
    [SerializeField] GameEvent OnSendReportAutomatically;
    [SerializeField] GameEvent OnEnableInput;
    [SerializeField] GameEvent OnDisableInput;
    [SerializeField] GameEvent OnNormalizeTime;
    [SerializeField] GameEvent OnSetTransitioningAnimTime;

    [Header("Game Overs")]
    [SerializeField] GameEvent OnTimeGameOver;
    [SerializeField] GameEvent OnAlertLevelGameOver;
    [SerializeField] GameEvent OnNotAgentsGameOver;
    bool isAPaperHolding;
    float currentDelay;
    ViewStates currentviewState;
    bool isInputDisable;
    bool isInPause;
    bool isGameOver;
    bool inOnFinalReport;
    bool isInPostGlitch;
    bool isInTutorial = false;
    bool delayingView;
    bool IsStuckInView;
    bool isTransitioning;
    bool isTimeAccelerated;
    ViewStates StuckView;
    ViewStates? nextViewRequest;

    Coroutine startViewCoroutine;
    private void Start()
    {
        
        currentDelay = delayBetweenViews;
        if (!isInTutorial)
        {
            OnDisableInput?.Invoke(this, null);
            startViewCoroutine = StartCoroutine(SetStartView(0.6f,0f));
        }
        else
        {
            UpdateViewState(null, ViewStates.TutorialView);
        }
    }

    IEnumerator SetStartView(float timeDelayView, float timeDelayEnableInput)
    {
        OnDisableInput?.Invoke(this, null);
        yield return new WaitForSeconds(timeDelayView);
        UpdateViewState(null, StartView);
        yield return new WaitForSeconds(timeDelayEnableInput);
        OnEnableInput?.Invoke(this, null);
    }

    public void SetStartView(Component sender, object obj)
    {
        if (startViewCoroutine != null) StopCoroutine(startViewCoroutine);

        StartViewData data = (StartViewData)obj;
        StartView = data.view;
        startViewCoroutine = StartCoroutine(SetStartView(data.ViewTime, data.InputDisableTime));
    }

    void Update()
    {
        if (isInputDisable)
        {
            //Debug.Log("disable");
            return;
        }

        //if (delayingView) return;

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (IsStuckInView && nextViewRequest != null) return;

            if (!isInTutorial)
            {
                if (currentviewState != ViewStates.GeneralView) CheckForBackToGeneralView();
                TimeManager.timeManager.NormalizeTime();
            }
            else
            {
                if (currentviewState != ViewStates.TutorialView ) CheckForBackToTutorialView();
            }

        }

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(2))
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

       /* if (Input.GetKeyDown(KeyCode.Alpha1))
            TimeGameOver();

        if (Input.GetKeyDown(KeyCode.Alpha2))
            AlertGameOver();

        if (Input.GetKeyDown(KeyCode.Alpha3))
            AnyAgentGameOver();*/
    }

    void CheckForBackToGeneralView()
    {
        if (inOnFinalReport) OnSendReportAutomatically?.Invoke(this, null);

        if (currentviewState == ViewStates.OnTakeSomeInBoard || currentviewState == ViewStates.BoardZoomView)
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

        if (currentviewState == ViewStates.OnTakeSomeInBoard || currentviewState == ViewStates.BoardZoomView)
        {
            UpdateViewState(this, ViewStates.BoardView);
            return;
        }
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
        //Se llama con OnViewChange 
        if (currentviewState == ViewStates.PauseView) return;
        if (currentviewState == ViewStates.OnTakeSomeInBoard) return;
        if (currentviewState == ViewStates.BoardZoomView) return;
        if (IsStuckInView)
        {
            nextViewRequest = StuckView;
            isTransitioning = false;
        }
    }

    public void UpdateViewState(Component sender, object _view)
    {
        //if (delayingView) return;
        ViewStates NewView = (ViewStates)_view;
        //StopAllCoroutines();
        /* if (NewView == currentviewState)
         {
             return;
         }*/
        if (isTransitioning)
        {
            // Guardamos el último pedido, reemplazando al anterior
            nextViewRequest = NewView;
            return;
        }

        StartCoroutine(HandleViewChange(NewView));
    }


    private IEnumerator HandleViewChange(ViewStates newView)
    {
        isTransitioning = true;

        // Llamamos al método original que hace la lógica completa
        ViewSelector(newView);

        // Tiempo mínimo antes de aceptar otra vista
        yield return new WaitForSeconds(currentDelay);

        if (nextViewRequest.HasValue)
        {
            ViewStates buffered = nextViewRequest.Value;
            nextViewRequest = null;
           StartCoroutine(HandleViewChange(buffered));
        }
        else
        {
            
            isTransitioning = false;
        }

    }

    void ViewSelector(ViewStates NewView)
    {
        if(IsStuckInView) if (NewView == currentviewState) return;
        switch (NewView)
        {
            case ViewStates.GeneralView:
                OnGeneralView?.Invoke(this, false);
                if (currentviewState == ViewStates.PCView) TimeManager.timeManager.NormalizeTime();
                if (currentviewState == ViewStates.TVView) TimeManager.timeManager.NormalizeTime();
                if (currentviewState == ViewStates.BoardView) TimeManager.timeManager.NormalizeTime();
                BackToGeneralViewWhitMoving();
                break;
            case ViewStates.PinchofonoView:
                if (isGameOver) return;
                if (inOnFinalReport) { OnSendReportAutomatically?.Invoke(this, null); return; }
                TimeManager.timeManager.NormalizeTime();
                OnNotebookTake.Invoke(this, true);
                OnPinchofonoView?.Invoke(this, null);
                break;
            case ViewStates.BoardView:
                if (isGameOver) return;
                if (inOnFinalReport) { OnSendReportAutomatically?.Invoke(this, null); return; }
                StartCoroutine(DelayForTimeChange(() => TimeManager.timeManager.PauseTime()));
                //TimeManager.timeManager.PauseTime();
                OnBoardView?.Invoke(this, null);
                OnNotebookTake.Invoke(this, true);
                break;
            case ViewStates.PCView:
                if (isGameOver) return;
                TimeManager.timeManager.SetAnotherSpeed(0.03125f);
                OnPCWiew?.Invoke(this, null);
                OnNotebookTake.Invoke(this, true);
                break;
            case ViewStates.ProgressorView:
                if (isGameOver) return;
                if (inOnFinalReport) { OnSendReportAutomatically?.Invoke(this, null); }
                OnProgressorView?.Invoke(this, null);
                break;
            case ViewStates.TVView:
                if (inOnFinalReport) { OnSendReportAutomatically?.Invoke(this, null); return; }
                TimeManager.timeManager.SetAnotherSpeed(0.0625f);
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
                BackToTakePaperViewWithMoving(NewView);
                break;
            case ViewStates.OnTakeSomeInBoard:
                if (isGameOver) return;
                if (inOnFinalReport) { OnSendReportAutomatically?.Invoke(this, null); return; }
                OnTakeSomeInBoard?.Invoke(this, null);
                break;
            case ViewStates.GameOverView:
                if (isInPostGlitch) return;
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
            case ViewStates.BoardZoomView:
                OnZoomView?.Invoke(this, null);
                OnNotebookTake.Invoke(this, false);
                break;

        }
        OnViewStateChange?.Invoke(this, NewView);
        OnSetTransitioningAnimTime?.Invoke(this, 0.5f);
        currentviewState = NewView;
        StartCoroutine(DelayBetweenViews());
        if (currentviewState != ViewStates.PauseView) if (isInPause) isInPause = false;
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
        if (currentviewState != ViewStates.GeneralView && currentviewState != ViewStates.DossierView && currentviewState != ViewStates.OnTakenPaperView  && !IsStuckInView)
        {
            if (isGameOver) return;
            if (isTimeAccelerated && currentviewState != ViewStates.PinchofonoView) return;
            OnSitDownSound?.Invoke(this, null);
        }

       
    }

    void BackToTakePaperViewWithMoving(ViewStates nextView)
    {
        if (IsAMovedPosition(currentviewState))
        {
            if (nextView == ViewStates.OnTakenPaperView) OnSitDownSound?.Invoke(this, null);
        }
    }

    bool IsAMovedPosition(ViewStates viewState)
    {
        return viewState == ViewStates.ProgressorView
            || viewState == ViewStates.TVView
            || viewState == ViewStates.PCView
            || viewState == ViewStates.PauseView
            || viewState == ViewStates.BoardView
            || viewState == ViewStates.PinchofonoView;
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
        isInPostGlitch = true;
    }

    public void OnFinalReportTakeFalse(Component sender, object obj)
    {
        inOnFinalReport = false;
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
        currentDelay = delayBetweenViews - 0.2f;
    }

    public void UnsetStuck(Component sender, object obj)
    {
        IsStuckInView = false;
        currentDelay = delayBetweenViews;
        nextViewRequest = null;
    }

    IEnumerator DelayForTimeChange(Action callback)
    {
        yield return new WaitForSeconds(0.5f);
        if (currentviewState == ViewStates.BoardView) callback?.Invoke();

    }

    public void OnTimeAccelerated(Component sender, object obj)
    {
        isTimeAccelerated = true;
    }

    public void OnTimeNormalized(Component sender, object obj)
    {
        isTimeAccelerated = false;
    }

    [ContextMenu("Trigger Time GameOver")]
    public void TimeGameOver()
    {
        OnTimeGameOver?.Invoke(this, null);
        UpdateViewState(this, ViewStates.GameOverView);
    }
    [ContextMenu("Trigger Alert GameOver")]
    public void AlertGameOver()
    {
        OnAlertLevelGameOver?.Invoke(this, null);
        UpdateViewState(this, ViewStates.GameOverView);
    }
    [ContextMenu("Trigger Any Agent GameOver")]
    public void AnyAgentGameOver()
    {
        OnNotAgentsGameOver?.Invoke(this, null);
        UpdateViewState(this, ViewStates.GameOverView);
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
    TutorialView,
    BoardZoomView
}

public class StartViewData
{
    public ViewStates view;
    public float ViewTime;
    public float InputDisableTime;
    public StartViewData(ViewStates _view,float _ViewTime, float _InputDisableTime)
    {
        view = _view;
        InputDisableTime = _InputDisableTime;
        ViewTime = _ViewTime;
    }
}
