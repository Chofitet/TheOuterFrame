using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Word Found Conditional", menuName = "Conditionals/WordFoundConditional")]
public class WordFoundConditional : ScriptableObject, IConditionable
{
    [SerializeField] WordData word;

    public bool CheckIfHaveTime()
    {
        return false;
    }

    public IConditionable GetLastCompletedConditional()
    {
        return this as IConditionable;
    }

    public bool GetStateCondition(int NumAlternativeCondition)
    {
        if (!word) Debug.LogWarning("the Word Found conditional " + name + " not have a word assigned");
        return word.GetIsFound();
    }

    public int GetTimeToShowNews()
    {
        throw new System.NotImplementedException();
    }

    public TimeData GetTimeWhenWasComplete()
    {
        throw new System.NotImplementedException();
    }
}
