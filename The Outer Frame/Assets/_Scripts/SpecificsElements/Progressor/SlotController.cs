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

    public void initParameters(WordData word, StateEnum state, int ActionDuration) 
    {
        gameObject.SetActive(true);
        actionDuration = ActionDuration;
        _word = word;
        _state = state;
        Wordtxt.text = word.GetName();
        Actiontxt.text = state.GetActionVerb();

        minuteProgress = 0;
        ProgressBar.maxValue = actionDuration;
        ProgressBar.value = 0;
        TimeManager.OnMinuteChange += UpdateProgress;
    }

    void UpdateProgress()
    {
        if (isActionComplete) return;
        minuteProgress += 1;
        ProgressBar.value = minuteProgress;

        if (minuteProgress >= actionDuration)
        {
            CompleteAction();
            isActionComplete = true;
        }
    }

    private void CompleteAction()
    {
        WordsManager.WM.RequestChangeState(_word, _state);
        AgentManager.AM.SetActiveOrDesactive(_state, true);
        OnFinishActionProgress?.Invoke(this, this);
        SetLEDState(Color.yellow);
    }

    public void ActionWasDone()
    {
        Wordtxt.text = "the action has already been done";
        Invoke("AbortAction", 3);
    }

    public void AbortAction()
    {
        OnFinishActionProgress?.Invoke(this, this);
        ResetSlot();
        AgentManager.AM.SetActiveOrDesactive(_state, true);
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

}
