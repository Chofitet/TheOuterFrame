using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New Call", menuName ="Calls")]
public class CallType : ScriptableObject, IStateComparable
{
    
    [Header("StartTime")]
    [SerializeField] int StartHour;
    [SerializeField] int StartMinute;
    [Header("EndTime")]
    [SerializeField] int EndHour;
    [SerializeField] int EndMinute;

    TimeCheckConditional StartTime;
    TimeCheckConditional EndTime;


    [Header("Reaction Call")]
    [SerializeField] StateEnum state;
    [SerializeField] int DelayToShowCallInMinutes = 30;
    [SerializeField] int CatchWindowInMinutes = 30;

    [Header("Common Properties")]
    [SerializeField] string From;
    [SerializeField] string To;
    [SerializeField] [TextArea(minLines: 3, maxLines: 10)] string Dialogue;
    [SerializeField] [TextArea(minLines: 3, maxLines: 10)] string IterruptedDialogue;

    [SerializeField] List<WordData> Involved = new List<WordData>(); 
    
    [SerializeField] List<ConditionalClass> Conditions = new List<ConditionalClass>();
    [SerializeField] bool isOrderMatters;

    [NonSerialized] private bool isCatch;
    [NonSerialized] private bool isInterrupted;
    [NonSerialized] WordData word;


    public string GetDialogue() {
        if (!isInterrupted) return Dialogue;
        else return IterruptedDialogue;
    }
    public void SetCached()
    {
        isCatch = true;
        SetTimeWhenWasDone();
    }
    public bool GetIsCatch() { return isCatch; }

    public string GetFrom() { return From; }
    public string GetTo() { return To; }

    public List<WordData> GetInvolved() { return Involved; }

    public void SetIsinterrrupted() { isInterrupted = true; }
    public StateEnum GetState() { return state; }

    public void SetWord(WordData _word)
    {
        word = _word;
        StartTime.Initialize(true, 0, StartHour, StartMinute);
        EndTime.Initialize(false, 0, EndHour, EndMinute);
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
