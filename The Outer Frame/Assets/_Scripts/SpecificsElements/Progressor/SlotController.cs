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

    [SerializeField] Image[] LEDObjects;
    int actionDuration;
    int minuteProgress;
    string _word;
    string _state;
    bool isActionComplete;
    ProgressorManager ProgressorReference;

    void OnEnable()
    {
        TimeManager.OnMinuteChange += UpdateProgress; 
    }

    void OnDisable()
    {
        TimeManager.OnMinuteChange -= UpdateProgress; 
    }


    public void initParameters(string word, string action, int ActionDuration, ProgressorManager reference) 
    {
        actionDuration = ActionDuration;
        _word = word;
        _state = action;
        Wordtxt.text = word;
        Actiontxt.text = action;
        ProgressorReference = reference;

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
        WordsManager.WM.RequestChangeState(_word, WordsManager.WM.ConvertStringToState(_state));
        ProgressorReference.ActionFinish(_word, gameObject);
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
    }


    void SetLEDState()
    {
        foreach (Image O in LEDObjects)
        {
            O.color = Color.yellow;
        }
    }


}
