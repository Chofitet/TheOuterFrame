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
    [SerializeField] GameEvent OnFindableWordsActive;
    [SerializeField] GameEvent OnTakeSomeInBoard;
    [SerializeField] GameEvent OnTakenPaperView;
    [SerializeField] GameEvent OnGameOverView;
    [SerializeField] GameEvent OnPauseView;
    [SerializeField] GameEvent OnBackToPause;
    [SerializeField] GameEvent OnSitDownSound;
    Coroutine StartDelay;
    bool isAPaperHolding;
    ViewStates currentviewState;
    bool isInputDisable;
    bool isInPause;

    private void Start()
    {
        Invoke("SetStartView", 0.6f);
    }

    void SetStartView()
    {
        UpdateViewState(null, StartView);
    }

    void Update()
    {
        if (isInputDisable) return;
        if (Input.GetKeyDown(KeyCode.Mouse1) && currentviewState != ViewStates.GeneralView)
        {
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
                OnFindableWordsActive?.Invoke(this, null);
                BackToGeneralViewWhitMoving();
                break;
            case ViewStates.PinchofonoView:
                OnNotebookTake.Invoke(this, true);
                OnPinchofonoView?.Invoke(this, null);
                break;
            case ViewStates.BoardView:
                TimeManager.timeManager.PauseTime();
                OnBoardView?.Invoke(this, null);
                OnNotebookTake.Invoke(this, true);
                OnFindableWordsActive?.Invoke(this, null);
                break;
            case ViewStates.PCView:
                OnPCWiew?.Invoke(this, null);
                OnNotebookTake.Invoke(this, true);
                OnFindableWordsActive?.Invoke(this, null);
                break;
            case ViewStates.ProgressorView:
                OnProgressorView?.Invoke(this, null);
                break;
            case ViewStates.TVView:
                OnTVView?.Invoke(this, null);
                OnNotebookTake.Invoke(this, true);
                OnFindableWordsActive?.Invoke(this, null);
                break;
            case ViewStates.DossierView:
                OnDossierView?.Invoke(this, null);
                OnNotebookTake.Invoke(this, true);
                OnFindableWordsActive?.Invoke(this, null);
                break;
            case ViewStates.OnTakenPaperView:
                OnTakenPaperView?.Invoke(this, null);
                OnNotebookTake.Invoke(this, true);
                OnFindableWordsActive?.Invoke(this, null);
                break;
            case ViewStates.OnTakeSomeInBoard:
                OnTakeSomeInBoard?.Invoke(this, null);
                break;
            case ViewStates.GameOverView:
                TimeManager.timeManager.NormalizeTime();
                OnGameOverView.Invoke(this, "RetryMenu");
                OnNotebookTake.Invoke(this, false);
                break;
            case ViewStates.PauseView:
                OnPauseView?.Invoke(this, null);
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
        if (currentviewState != ViewStates.DossierView && currentviewState != ViewStates.OnTakenPaperView)
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
}
