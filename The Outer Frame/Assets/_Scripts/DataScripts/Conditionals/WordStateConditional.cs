using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New WordStateConditional", menuName = "Conditionals/WordStateConditional")]
public class WordStateConditional : ScriptableObject, IConditionable
{
    [SerializeField] WordData word;
    [SerializeField] StateEnum state;


    public bool CheckIfHaveTime()
    {
        return true;
    }
    public bool GetStateCondition()
    {
        return WordsManager.WM.CheckIfStateWasDone(word, state);
    }

    public bool GetAlternativeConditional()
    {
        return WordsManager.WM.CheckIfStateSeenWasDone(word, state);
    }

    public TimeData GetTimeWhenWasComplete()
    {
        return WordsManager.WM.RequestTimeDataOfState(word, state);
    }
}
