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
    TimeData timeComplete;
    bool inFillFast;
    StateEnum SpecialState;
    ReportType Report;

    bool isfillSmooth;

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
            state.SetIsDone(true);
        }
        Wordtxt.GetComponent<WarpTextExample>().UpdateText();
        Actiontxt.text = state.GetActioningVerb();
        if(state.GetSpecialActionWord()) Actiontxt.text = state.GetIdeaVerb();
        Actiontxt.GetComponent<WarpTextExample>().UpdateText();
        isAborted = false;
        isAlreadyDone = false;

        minuteProgress = 0;
        ProgressBar.maxValue = actionDuration;
        ProgressBar.value = 0;
        

        if (Report.GetWasSet())
        {
            inFillFast = true;
            ProgressBar.maxValue = 1.5f;
            isAlreadyDone = true;
        }
        else if(Report.GetIsAutomatic())
        {
            inFillFast = true;
            ProgressBar.maxValue = 1.5f;
            isAutomaticAction = true;
        }
        else
        {
            TimeManager.OnMinuteChange += UpdateProgress;
            UpdateProgress();
        }
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
        float animationDuration = 60/TimeManager.timeManager.GetActuaTimeVariationSpeed();
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

    }

    private void CompleteAction()
    {
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
        isAborted = true;
        OnFinishActionProgress?.Invoke(this, this);
        SetLEDState(Color.yellow);
        TimeManager.OnMinuteChange -= UpdateProgress;
        timeComplete = TimeManager.timeManager.GetTime();
        Icon.SetActive(true);
        if (_state.GetSpecialActionWord()) _state.SetIsDone(false);
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

    public WordData GetWord()
    {
        return _word;
    }

    public StateEnum GetState()
    {
        return _state;
    }

    public ReportType GetReport()
    {
        return Report;
    }

    public bool GetIsAborted()
    {
        return isAborted;
    }

    public bool getisAlreadyDone()
    {
        return isAlreadyDone;
    }

    public TimeData GetTimeComplete()
    {
        return timeComplete;
    }
}
