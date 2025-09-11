using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New WordStateConditional", menuName = "Conditionals/WordStateConditional")]
public class WordStateConditional : ScriptableObject, IConditionable
{
    [SerializeField] WordData word;
    [SerializeField] StateEnum state;
    [SerializeField] int TimeToAppearOnTV;

    public void SetWord(WordData x) => word = x;
    public void SetState(StateEnum x) => state = x;

    public bool CheckIfHaveTime()
    {
        return true;
    }
    public bool GetStateCondition(int alternativeConditional = 1)
    {
        switch (alternativeConditional)
        {
            case 1: return WordsManager.WM.CheckIfStateWasDone(word, state);
            case 2: return WordsManager.WM.CheckIfStateSeenWasDone(word, state);
            case 3: return WordsManager.WM.CheckIfStateSeenWasEntryInDB(word, state);
        }

       return WordsManager.WM.CheckIfStateWasDone(word, state);
    }

    public TimeData GetTimeWhenWasComplete()
    {
        return WordsManager.WM.RequestTimeDataOfState(word, state);
    }

    public IConditionable GetLastCompletedConditional()
    {
        return this as IConditionable;
    }

    public int GetTimeToShowNews()
    {
        return TimeToAppearOnTV;
    }
}
