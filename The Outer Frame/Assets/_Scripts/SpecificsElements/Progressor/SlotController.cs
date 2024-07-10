using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class SlotController : MonoBehaviour
{
    [SerializeField] TMP_Text Wordtxt;
    [SerializeField] TMP_Text Actiontxt;
    [SerializeField] Slider ProgressBar;
    [SerializeField] GameEvent OnFinishActionProgress;
 
    [SerializeField] Image[] LEDObjects;
    int actionDuration;
    int minuteProgress;
    WordData _word;
    StateEnum _state;
    bool isActionComplete;
    bool isAborted;
    bool isNotPossible;
    bool inFillFast;

    public void initParameters(WordData word, StateEnum state, int ActionDuration) 
    {
        gameObject.SetActive(true);
        actionDuration = ActionDuration;
        _word = word;
        _state = state;
        Wordtxt.text = word.GetName();
        Actiontxt.text = state.GetActionVerb();
        isAborted = false;
        isNotPossible = false;

        minuteProgress = 0;
        ProgressBar.maxValue = actionDuration;
        ProgressBar.value = 0;
        

        if (WordsManager.WM.CheckIfStateWasDone(word, state))
        {
            inFillFast = true;
            ProgressBar.maxValue = 1.5f;
            isNotPossible = true;
        }
        else
        {
            TimeManager.OnMinuteChange += UpdateProgress;
        }
    }

    void UpdateProgress()
    {
        if (isActionComplete) return;
        minuteProgress += 1;
        ProgressBar.value = minuteProgress;

        if (minuteProgress >= actionDuration)
        {
            if(isNotPossible)
            {
                ActionWasDone();
            }
            else CompleteAction();
            isActionComplete = true;
        }
    }

    private void Update()
    {
        if (inFillFast && ProgressBar.value <= ProgressBar.maxValue)
        {
            ProgressBar.value += Time.deltaTime;
            Debug.Log(ProgressBar.value);
            Debug.Log(ProgressBar.maxValue);
        }
        
        if (inFillFast && ProgressBar.value == ProgressBar.maxValue)
        {
            Debug.Log("full");
            ActionWasDone();
        }

    }

    private void CompleteAction()
    {
        WordsManager.WM.RequestChangeState(_word, _state);
        AgentManager.AM.SetActiveOrDesactive(_state, true);
        OnFinishActionProgress?.Invoke(this, this);
        SetLEDState(Color.yellow);
    }

    void ActionWasDone()
    {
        inFillFast = false;
        OnFinishActionProgress?.Invoke(this, this);
        AgentManager.AM.SetActiveOrDesactive(_state, true);
        SetLEDState(Color.red);
        TimeManager.OnMinuteChange -= UpdateProgress;
        ProgressBar.value = 0;
    }

    public void AbortAction()
    {
        isAborted = true;
        OnFinishActionProgress?.Invoke(this, this);
        AgentManager.AM.SetActiveOrDesactive(_state, true);
        SetLEDState(Color.yellow);
        TimeManager.OnMinuteChange -= UpdateProgress;
        ProgressBar.value = 0;
    }

    public void CleanSlot()
    {
        ResetSlot();
        WordsManager.WM.RequestChangeStateSeen(_word,_state);
    }

    void ResetSlot()
    {
        gameObject.SetActive(false);
        isActionComplete = false;
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

    public bool IsActionComplete()
    {
        return isActionComplete;
    }
    public bool GetIsAborted()
    {
        return isAborted;
    }
    public bool GetIsNotPossible()
    {
        return isNotPossible;
    }

}
