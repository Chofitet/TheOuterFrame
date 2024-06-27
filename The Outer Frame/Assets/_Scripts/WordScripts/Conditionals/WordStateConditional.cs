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
        return WordsManager.WM.CheckIfStateWasDone(word.GetName(), state);
    }

    public TimeData GetTimeWhenWasComplete()
    {
        return WordsManager.WM.RequestTimeDataOfState(word.GetName(), state);
    }
}
