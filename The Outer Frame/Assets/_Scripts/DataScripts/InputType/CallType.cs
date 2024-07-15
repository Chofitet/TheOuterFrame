using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New Call", menuName ="Input/Calls")]
public class CallType : ScriptableObject, IStateComparable
{
    
    [Header("Pre-programmed Call")]
    [SerializeField] TimeCheckConditional StartTime;
    [SerializeField] TimeCheckConditional EndTime;


    [Header("Reaction Call")]
    [SerializeField] StateEnum state;
    [SerializeField] int DelayToSchowCallInMinutes = 30;
    [SerializeField] int CatchWindowInMinutes = 30;

    [Header("Common Properties")]
    [SerializeField] [Multiline] string Dialogue;
    [SerializeField] List<ScriptableObject> Conditionals = new List<ScriptableObject>();
    [SerializeField] bool isOrderMatters;

    [NonSerialized] private bool isCatch;

    public string GetDialogue() { return Dialogue; }
    public void SetCached() => isCatch = true;
    public bool GetIsCatch() { return isCatch; }
    public StateEnum GetState() { return state; }

    public void DefineTimeZone()
    {
        TimeData ActualTime = TimeManager.timeManager.GetTime();

        TimeData FixedStartTime = AddMinutesToTime(ActualTime, DelayToSchowCallInMinutes);
        StartTime = ScriptableObject.CreateInstance<TimeCheckConditional>();
        StartTime.Initialize(true, FixedStartTime.Day, FixedStartTime.Hour, FixedStartTime.Minute);

        TimeData FixedEndTime = AddMinutesToTime(ActualTime, DelayToSchowCallInMinutes + CatchWindowInMinutes);
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

        foreach (ScriptableObject conditional in Conditionals)
        {
            if (conditional is not IConditionable)
            {
                Debug.LogWarning(conditional.name + " is not a valid conditional");
                return false; 
            }

            IConditionable auxConditional = conditional as IConditionable;

            if (!auxConditional.GetStateCondition())
            {
                return false;
            }
        }

        if (isOrderMatters) return CheckIfConditionalAreInOrder();
        else return true;
    }

    bool CheckIfConditionalAreInOrder()
    {
        List<int> nums = new List<int>();

        foreach (ScriptableObject conditional in Conditionals)
        {
            IConditionable auxConditional = conditional as IConditionable;

            if (auxConditional.CheckIfHaveTime())
            {
                nums.Add(auxConditional.GetTimeWhenWasComplete().GetTimeInNum());
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
