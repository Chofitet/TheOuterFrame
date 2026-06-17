using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Appear On Report Conditional", menuName = "Conditionals/Appear On Report Conditional")]
public class AppearOnReportConditional : ScriptableObject, IConditionable
{

    [SerializeField] WordData word;
    public bool CheckIfHaveTime()
    {
        return false;
    }

    public IConditionable GetLastCompletedConditional()
    {
        throw new System.NotImplementedException();
    }

    public bool GetStateCondition(int NumOfAlternativeConditional = 1)
    {
        return word.GetAppearOnReport();
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
