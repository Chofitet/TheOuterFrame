using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New Call", menuName ="Calls")]
public class CallType : ScriptableObject, IStateComparable
{
    
    [Header("Pre-programmed Call")]
    [SerializeField] TimeCheckConditional StartTime;
    [SerializeField] TimeCheckConditional EndTime;


    [Header("Reaction Call")]
    [SerializeField] StateEnum state;
    [SerializeField] int DelayToShowCallInMinutes = 30;
    [SerializeField] int CatchWindowInMinutes = 30;

    [Header("Common Properties")]
    [SerializeField] [Multiline] string Dialogue;
    [SerializeField] List<ConditionalClass> Conditions = new List<ConditionalClass>();
    [SerializeField] bool isOrderMatters;

    [NonSerialized] private bool isCatch;
    [NonSerialized] WordData word;

    public string GetDialogue() { return Dialogue; }
    public void SetCached()
    {
        isCatch = true;
        SetTimeWhenWasDone();
    }
    public bool GetIsCatch() { return isCatch; }
    public StateEnum GetState() { return state; }

    public void SetWord(WordData _word)
    {
        word = _word;
    }

    public WordData GetWord() { return word; }

    private TimeData CompleteTime;
    public void SetTimeWhenWasDone()
    {
        CompleteTime = TimeManager.timeManager.GetTime();
    }

    public TimeData GetTimeWhenWasDone() { return CompleteTime; }

    public void DefineTimeZone()
    {
        TimeData ActualTime = TimeManager.timeManager.GetTime();

        TimeData FixedStartTime = AddMinutesToTime(ActualTime, DelayToShowCallInMinutes);
        StartTime = ScriptableObject.CreateInstance<TimeCheckConditional>();
        StartTime.Initialize(true, FixedStartTime.Day, FixedStartTime.Hour, FixedStartTime.Minute);

        TimeData FixedEndTime = AddMinutesToTime(ActualTime, DelayToShowCallInMinutes + CatchWindowInMinutes);
        EndTime = ScriptableObject.CreateInstance<TimeCheckConditional>();
        EndTime.Initialize(false, FixedEndTime.Day, FixedEndTime.Hour, FixedEndTime.Minute);

        Debug.Log("Defined Time Zone. Start: " + FixedStartTime.ToString() + " Ends: " + FixedEndTime.ToString());
    }

    private TimeData AddMinutesToTime(TimeData time, int minutesToAdd)
    {
        int totalMinutes = time.Minute + minutesToAdd;
        int extraHours = totalMinutes / 60;
        int finalMinutes = totalMinutes % 60;

        int totalHours = time.Hour + extraHours;
        int extraDays = totalHours / 24;
        int finalHours = totalHours % 24;

        int finalDays = time.Day + extraDays;

        return new TimeData
        {
            Day = finalDays,
            Hour = finalHours,
            Minute = finalMinutes
        };
    }

    public bool CheckForTimeZone()
    {
        if (!StartTime || !EndTime) return false;
        if (StartTime.GetStateCondition() && EndTime.GetStateCondition()) return true;
        else return false;
    }

    public bool CheckForConditionals()
    {

        if (Conditions == null) return true;

        foreach (ConditionalClass conditional in Conditions)
        {
            IConditionable auxInterface = conditional.condition as IConditionable;

            bool conditionState = auxInterface.GetStateCondition();

            if (!conditional.ifNot)
            {
                conditionState = !conditionState;
            }

            if (conditionState)
            {
                return false;
            }
        }

        if (isOrderMatters) return CheckIfConditionalAreInOrder();
        else
        {
            return true;
        }
    }

    bool CheckIfConditionalAreInOrder()
    {
        List<int> nums = new List<int>();

        foreach (ConditionalClass conditional in Conditions)
        {
            IConditionable auxInterface = conditional.condition as IConditionable;

            if (auxInterface.CheckIfHaveTime())
            {
                nums.Add(auxInterface.GetTimeWhenWasComplete().GetTimeInNum());
            }

        }

        for (int i = 0; i < nums.Count - 1; i++)
        {
            if (nums[i] > nums[i + 1])
            {
                return false;
            }
        }

        return true;
    }

}
