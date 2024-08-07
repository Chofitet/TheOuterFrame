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
    [SerializeField] GameEvent OnPrinterView;
    [SerializeField] GameEvent OnViewStateChange;
    [SerializeField] GameEvent OnNotebookTake;
    [SerializeField] GameEvent OnNotebookLeave;
    [SerializeField] GameEvent OnCallTranscriptionView;
    [SerializeField] GameEvent OnFindableWordsActive;
    [SerializeField] GameEvent OnTakeSomeInBoard;
    Coroutine StartDelay;
    ViewStates currentviewState;
    bool isReady = true;

    private void Start()
    {
        UpdateViewState(null, StartView);
    }

    void Update()
    {
        if (!isReady) return;
        if (Input.GetKeyDown(KeyCode.Mouse1) && currentviewState != ViewStates.GeneralView)
        {
            if(currentviewState == ViewStates.OnTakeSomeInBoard)
            {
                UpdateViewState(this, ViewStates.BoardView);
                return;
            }

            BackToGeneralView(null, null);
        }
    }

    public void BackToGeneralView(Component sender, object _view)
    {
        if (!isReady) return;
        OnNotebookLeave?.Invoke(this, null);
        UpdateViewState(this, ViewStates.GeneralView);
    }

    public void UpdateViewState(Component sender, object _view)
    {
        if (!isReady) return;
        ViewStates NewView = (ViewStates)_view;
        if (NewView == currentviewState) return;
        switch (NewView)
        {
            case ViewStates.GeneralView:
                OnGeneralView?.Invoke(this, false);
                OnFindableWordsActive?.Invoke(this, null);
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
            case ViewStates.PrinterView:
                OnPrinterView?.Invoke(this, null);
                OnNotebookTake.Invoke(this, true);
                OnFindableWordsActive?.Invoke(this, null);
                break;
            case ViewStates.OnCallTranscriptionView:
                OnCallTranscriptionView?.Invoke(this, null);
                OnFindableWordsActive?.Invoke(this, null);
                break;
            case ViewStates.OnTakeSomeInBoard:
                OnTakeSomeInBoard?.Invoke(this, null);
                break;
        }
        OnViewStateChange?.Invoke(this,NewView);
        currentviewState = NewView;
        Debug.Log(currentviewState);
        if(isReady) StartDelayCoroutine(DelayBetweenViews);
    }

    public ViewStates GiveCurrentViewState()
    {
        return currentviewState;
    }

    public void StartDelayEvent(Component sender, object obj)
    {
        float delay = (float)obj;

        StartDelayCoroutine(delay);
    }

    void StartDelayCoroutine(float delay)
    {
        if (StartDelay == null)
        {
            StartDelay = StartCoroutine(Delay(delay));
        }
    }

    IEnumerator Delay(float delay)
    {
        isReady = false;
        yield return new WaitForSeconds(delay);
        isReady = true;
        StartDelay = null;
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
    OnTakeSomeInBoard,
}
