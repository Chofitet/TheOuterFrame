using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using TMPro.Examples;
using DG.Tweening;
using System.Linq;

public class SlotController : MonoBehaviour
{
    [SerializeField] TMP_Text Wordtxt;
    [SerializeField] TMP_Text Actiontxt;
    [SerializeField] Slider ProgressBar;
    [SerializeField] GameObject Icon;
    [SerializeField] GameEvent OnFinishActionProgress;

    [SerializeField] Image[] LEDObjects;
    int actionDuration;
    int minuteProgress;
    WordData _word;
    StateEnum _state;
    bool isActionComplete;
    bool isAborted;
    bool isAlreadyDone;
    bool isAutomaticAction;
    bool isTheSameAction;
    bool isOtherGroupActionDoing;
    TimeData timeComplete;
    bool inFillFast;
    ReportType Report;


    public void initParameters(WordData word, StateEnum state)
    {
        gameObject.SetActive(true);
        _word = word;
        Report = WordsManager.WM.RequestReport(word, state);
        _state = state;
        actionDuration = state.GetTime() + Report.GetChangeTimeOfAction();

        Wordtxt.text = word.GetProgressorNameVersion();
        if (state.GetSpecialActionWord())
        {
            Wordtxt.text = state.GetSpeticialActionWordName();
        }
        Wordtxt.GetComponent<WarpTextExample>().UpdateText();
        Actiontxt.text = state.GetActioningVerb();
        if (state.GetSpecialActionWord()) Actiontxt.text = state.GetIdeaVerb();
        Actiontxt.GetComponent<WarpTextExample>().UpdateText();
        isAborted = false;
        isAlreadyDone = false;

        minuteProgress = 0;
        ProgressBar.maxValue = actionDuration;
        ProgressBar.value = 0;


        //ya fue hecho
        if (Report.GetWasSet())
        {
            FillFast();
            isAlreadyDone = true;
        }
        //Se está haciendo el mismo en este momento
        else if (word.CheckIfActionIsDoing(state))
        {
            FillFast();
            isTheSameAction = true;
            Debug.Log("misma palabra + acción");
        }
        // Se está haciendo uno del mismo ActionGroup
        else if (ActionGroupManager.AGM.ChekAreInTheSameGroup(word, state))
        {
            FillFast();
            isOtherGroupActionDoing = true;
            Debug.Log("Otra acción del grupo en proceso");
        }
        // Es una acción automática
        else if (Report.GetIsAutomatic())
        {
            FillFast();
            isAutomaticAction = true;
        }
        // Es una acción válida
        else
        {
            word.SetDoingAction(state, true);
            TimeManager.OnMinuteChange += UpdateProgress;
            UpdateProgress();
        }
    }

    void FillFast()
    {
        inFillFast = true;
        ProgressBar.maxValue = 1.5f;
    }

    private Tween progressTween;

    void UpdateProgress()
    {
        if (isActionComplete) return;

        minuteProgress += 1;

        if (progressTween != null && progressTween.IsActive())
        {
            progressTween.Kill();
        }
        float animationDuration = 60 / TimeManager.timeManager.GetActuaTimeVariationSpeed();
        progressTween = ProgressBar.DOValue(minuteProgress, animationDuration);

        if (minuteProgress > actionDuration)
        {
            if (isAlreadyDone)
            {
                AutomaticAction();
            }
            else
            {
                CompleteAction();
            }
            isActionComplete = true;
        }
    }

    private void Update()
    {
        if (inFillFast && ProgressBar.value <= ProgressBar.maxValue)
        {
            ProgressBar.value += Time.deltaTime;
        }

        if (inFillFast && ProgressBar.value == ProgressBar.maxValue && isAlreadyDone)
        {
            AutomaticAction();
        }
        else if (inFillFast && ProgressBar.value == ProgressBar.maxValue && isAutomaticAction)
        {
            //WordsManager.WM.RequestChangeState(_word, _state);
            // _state = WordsManager.WM.GetHistory(_word).Last();
            AutomaticAction();
        }
        else if (inFillFast && ProgressBar.value == ProgressBar.maxValue && isTheSameAction)
        {
            AutomaticAction();
        }
        else if (inFillFast && ProgressBar.value == ProgressBar.maxValue && isOtherGroupActionDoing)
        {
            AutomaticAction();
        }

    }

    private void CompleteAction()
    {
        _word.SetDoingAction(_state, false);
        inFillFast = false;
        Report = WordsManager.WM.RequestReport(_word, _state);
        if (!Report.GetWasSet())
        {
            isAlreadyDone = false;
            WordsManager.WM.RequestChangeState(_word, Report);
            Report.SetWasSet();
        }
        else isAlreadyDone = true;
        _state = WordsManager.WM.GetHistory(_word).Last();
        Report.SetTimeWhenWasDone();
        timeComplete = TimeManager.timeManager.GetTime();
        OnFinishActionProgress?.Invoke(this, this);
        Icon.SetActive(true);
        if (_state.GetSpecialActionWord()) _state.SetIsDone(true);
    }


    void AutomaticAction()
    {
        inFillFast = false;
        OnFinishActionProgress?.Invoke(this, this);
        TimeManager.OnMinuteChange -= UpdateProgress;
        timeComplete = TimeManager.timeManager.GetTime();
        Icon.SetActive(true);
        SetLEDState(Color.green);
    }

    public void AbortAction()
    {
        _word.SetDoingAction(_state, false);
        isAborted = true;
        OnFinishActionProgress?.Invoke(this, this);
        SetLEDState(Color.yellow);
        TimeManager.OnMinuteChange -= UpdateProgress;
        timeComplete = TimeManager.timeManager.GetTime();
        Icon.SetActive(true);
    }

    public void CleanSlot()
    {
        ResetSlot();
    }

    void ResetSlot()
    {
        gameObject.SetActive(false);
        Icon.SetActive(false);
        Report = null;
        isActionComplete = false;
        ProgressBar.value = 0;
        TimeManager.OnMinuteChange -= UpdateProgress;
        SetLEDState(Color.green);
    }

    void SetLEDState(Color _color)
    {
        foreach (Image O in LEDObjects)
        {
            O.color = _color;
        }
    }

    public WordData GetWord() { return _word; }

    public StateEnum GetState() { return _state; }

    public ReportType GetReport() { return Report; }

    public bool GetIsAborted() { return isAborted; }

    public bool getisAlreadyDone() { return isAlreadyDone; }

    public bool GetIsTheSameAction() { return isTheSameAction; }

    public bool GetIsOtherGroupActionDoing() {return isOtherGroupActionDoing;}

    public TimeData GetTimeComplete() { return timeComplete;}

}
