using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Time Check Conditional", menuName = "Conditionals/TimeCheckConditional")]
public class TimeCheckConditional : ScriptableObject, IConditionable
{

    [SerializeField] WhenCondition Is;

    [Header("this time:")]

    [SerializeField] int Day;
    [SerializeField] int Hour = 00;
    [SerializeField] int Minute = 00;

    public void Initialize(bool isAfter, int _day , int _hour, int _minute)
    {
        Is = isAfter ? WhenCondition.Onwards : WhenCondition.Backwards;
        Day = _day;
        Hour = _hour;
        Minute = _minute;
    }

    private enum WhenCondition
    {
        Backwards,
        Onwards
    }


    public bool CheckIfHaveTime()
    {
        return false;
    }

    public bool GetStateCondition()
    {
        int timeToCheck = new TimeData(Day, Hour, Minute).GetTimeInNum();
        int ActualTime = TimeManager.timeManager.GetTime().GetTimeInNum();

        if (Is == WhenCondition.Backwards)
        {
            if (timeToCheck > ActualTime) return true;
            else return false; 
        }
        else
        {
            if (timeToCheck < ActualTime) return true;
            else return false;
        }
    }

    public TimeData GetTimeWhenWasComplete()
    {
        throw new System.NotImplementedException();
    }

    public bool GetAlternativeConditional()
    {
        return GetStateCondition();
    }
}
