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

    void OnEnable()
    {
        TimeManager.OnMinuteChange += UpdateProgress; 
    }

    void OnDisable()
    {
        TimeManager.OnMinuteChange -= UpdateProgress; 
    }


    public void initParameters(WordData word, StateEnum state, int ActionDuration) 
    {
        actionDuration = ActionDuration;
        _word = word;
        _state = state;
        Wordtxt.text = word.GetName();
        Actiontxt.text = state.GetActionVerb();

        ProgressBar.maxValue = actionDuration;
        ProgressBar.value = 0;
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
        SetLEDState();
    }

    public void ActionWasDone()
    {
        TimeManager.OnMinuteChange -= UpdateProgress;
        Wordtxt.text = "the action has already been done";
        Invoke("AbortAction", 3);
    }

    public void AbortAction()
    {
        Destroy(gameObject);
        AgentManager.AM.SetActiveOrDesactive(_state, true);
    }

    public void CleanSlot()
    {
        WordsManager.WM.RequestChangeStateSeen(_word,_state);
        Destroy(gameObject);
    }


    void SetLEDState()
    {
        foreach (Image O in LEDObjects)
        {
            O.color = Color.yellow;
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
