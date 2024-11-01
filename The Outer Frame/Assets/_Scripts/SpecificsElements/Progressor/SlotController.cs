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
    [SerializeField] GameObject AgentIcon;
    [SerializeField] GameEvent OnFinishActionProgress;
    

    [SerializeField] Image[] LEDObjects;
    int actionDuration;
    int secondProgress;
    WordData _word;
    StateEnum _state;
    bool isActionComplete;
    bool isAborted;
    bool isAlreadyDone;
    bool isAutomaticAction;
    bool isTheSameAction;
    StateEnum isOtherGroupActionDoing;
    TimeData timeComplete;
    bool inFillFast;
    ReportType Report;


    public void initParameters(WordData word, StateEnum state)
    {
        gameObject.SetActive(true);
        transform.GetChild(0).gameObject.SetActive(true);
        AgentIcon.SetActive(false);
        _word = word;
        Report = WordsManager.WM.RequestReport(word, state);
        _state = state;
        actionDuration = (state.GetTime() + Report.GetChangeTimeOfAction())*60;

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

        secondProgress = 0;
        ProgressBar.maxValue = actionDuration;
        ProgressBar.value = 0;


        //ya fue hecho
        if (Report.GetWasSet())
        {
            FillFast();
            isAlreadyDone = true;
            SetLEDState(Color.red);
        }
        //Se está haciendo el mismo en este momento
        else if (word.CheckIfActionIsDoing(state))
        {
            FillFast();
            isTheSameAction = true;
            Debug.Log("misma palabra + acción");
            SetLEDState(Color.red);
        }
        // Se está haciendo uno del mismo ActionGroup
        else if (ActionGroupManager.AGM.ChekAreInTheSameGroup(word, state))
        {
            FillFast();
            isOtherGroupActionDoing = _word.GetDoingAction(0);
            Debug.Log("Otra acción del grupo en proceso");
            SetLEDState(Color.red);
        }
        // Es una acción automática
        else if (Report.GetIsAutomatic())
        {
            FillFast();
            isAutomaticAction = true;
            SetLEDState(Color.red);
        }
        // Es una acción válida
        else
        {
            word.SetDoingAction(state, true);
            TimeManager.OnSecondsChange += UpdateProgress;
            UpdateProgress();
            SetLEDState(Color.green);
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

        secondProgress += 1;

        ProgressBar.value = secondProgress;

        if (secondProgress > actionDuration)
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
        else if (inFillFast && ProgressBar.value == ProgressBar.maxValue && isOtherGroupActionDoing != null)
        {
            AutomaticAction();
        }

    }

    private void CompleteAction()
    {
        _word.SetDoingAction(_state, false);
        inFillFast = false;
        Report = WordsManager.WM.RequestReport(_word, _state);
        AgentIcon.SetActive(false);
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
        if (_state.GetSpecialActionWord()) _state.SetIsDone(true);
    }


    void AutomaticAction()
    {
        inFillFast = false;
        OnFinishActionProgress?.Invoke(this, this);
        AgentIcon.SetActive(false);
        TimeManager.OnMinuteChange -= UpdateProgress;
        timeComplete = TimeManager.timeManager.GetTime();
    }

    public void AbortAction()
    {
        _word.SetDoingAction(_state, false);
        isAborted = true;
        inFillFast = false;
        OnFinishActionProgress?.Invoke(this, this);
        TimeManager.OnSecondsChange -= UpdateProgress;
        timeComplete = TimeManager.timeManager.GetTime();

        Icon.SetActive(true);
    }

    public void CleanSlot()
    {
        ResetSlot();
    }

    void ResetSlot()
    {
        Icon.SetActive(false);
        
        isActionComplete = false;
        isOtherGroupActionDoing = null;
        ProgressBar.value = 0;
        TimeManager.OnSecondsChange -= UpdateProgress;
        SetLEDState(Color.green);
        AgentIcon.SetActive(true);
        if (Report.GetKillAgent()) DisableAgent();
        Report = null;
        transform.GetChild(0).gameObject.SetActive(false);
    }

    void SetLEDState(Color _color)
    {
        foreach (Image O in LEDObjects)
        {
            O.color = _color;
        }
    }

    public void TurnOffProgressor(Component sender, object obj)
    {
        Icon.SetActive(false);
        Report = null;
        ProgressBar.value = 0;
        TimeManager.OnSecondsChange -= UpdateProgress;
        Wordtxt.text = "";
        Actiontxt.text = "";
        SetLEDState(Color.black);
    }

    void DisableAgent()
    {
        AgentIcon.GetComponent<Image>().color = Color.red;
        AgentIcon.GetComponent<RectTransform>().Rotate(new Vector3(0, 0, 90));
    }

    public WordData GetWord() { return _word; }

    public StateEnum GetState() { return _state; }

    public ReportType GetReport() { return Report; }

    public bool GetIsAborted() { return isAborted; }

    public bool getisAlreadyDone() { return isAlreadyDone; }

    public bool GetIsTheSameAction() { return isTheSameAction; }

    public StateEnum GetIsOtherGroupActionDoing() {return isOtherGroupActionDoing;}

    public TimeData GetTimeComplete() { return timeComplete;}

}
