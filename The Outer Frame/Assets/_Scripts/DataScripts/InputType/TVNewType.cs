using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New TVNew", menuName = "News/ReactiveNew")]
public class TVNewType : ScriptableObject, IStateComparable, INewType, IReseteableScriptableObject
{
    [HideInInspector][SerializeField] StateEnum state;
    [SerializeField] [TextArea(minLines: 3, maxLines: 10)] string headline;
    [TextArea(minLines: 3, maxLines: 10)] [SerializeField] string headlineTwoLines;
    [SerializeField] [TextArea(minLines: 3, maxLines: 10)] string text;
    [SerializeField] Sprite image;
    [SerializeField] int channel;
    [SerializeField] int priority = 5;
    [SerializeField] int alertLevelIncrement;
    [SerializeField] bool Emergency;
    [SerializeField] int TimeToAppear;
    [SerializeField] int MinTransmitionTime = 8;
    [SerializeField] List<ConditionalClass> Conditions = new List<ConditionalClass>();
    [NonSerialized] bool wasStremed;
    bool isOrderMatters;

    private void OnEnable()
    {
        ScriptableObjectResetter.instance?.RegisterScriptableObject(this);
    }

    public void ResetScriptableObject()
    {
        wasStremed = false;
    }

    public StateEnum GetState()
    {
        return state;
    }

    private TimeData CompleteTime;
    public void SetTimeWhenWasDone()
    {
        CompleteTime = TimeManager.timeManager.GetTime();
    }

    public string GetHeadline() { return headline; }
    public TimeData GetTimeWhenWasDone() { return CompleteTime;}

    public Sprite GetNewImag(){return image;}

    public bool GetIfIsAEmergency(){ return Emergency;}

    public void AddConditional(ConditionalClass condition)
    {
        Conditions.Add(condition);
    }
    public void removeCondition()
    {
        if (Conditions[0] == null) return;
        Conditions.RemoveAt(0);
    }
    public int GetIncreaseAlertLevel()
    {
        return alertLevelIncrement;
    }

    public string GetNewText()
    {
        return text;
    }

    public bool GetStateConditionalToAppear()
    {
        if (CheckConditionals() && !EndTime)
        {
            DefineTimeZone();
            return false;
        }

        if (!EndTime){return false;}
        
        if(EndTime.GetStateCondition()) return true;

        return false;

        
    }

    public bool CheckConditionals()
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

    TimeCheckConditional EndTime = null;
     void DefineTimeZone()
    {
        TimeData ActualTime = TimeManager.timeManager.GetTime();

        TimeData FixedEndTime = AddMinutesToTime(ActualTime, GetTimeToAppear());
        EndTime = ScriptableObject.CreateInstance<TimeCheckConditional>();
        EndTime.Initialize(true, FixedEndTime.Day, FixedEndTime.Hour, FixedEndTime.Minute);

        Debug.Log("Defined Time to show new: " + FixedEndTime.ToString());
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

    public int GetTimeToAppear()
    {
        if (TimeToAppear != 0) return TimeToAppear;

        IConditionable lastCompleteConditional = null;
        int latestTime = 0;

        if (Conditions.Count == 1)
        {
            IConditionable oneCondition = Conditions[0].condition as IConditionable;
            return oneCondition.GetLastCompletedConditional().GetTimeToShowNews();
        }

            foreach (ConditionalClass conditional in Conditions)
        {
            IConditionable auxConditional = conditional.condition as IConditionable;

                int completionTime = auxConditional.GetLastCompletedConditional().GetTimeWhenWasComplete().GetTimeInNum();

                foreach (ConditionalClass otherConditional in Conditions)
                {
                    IConditionable otherAuxConditional = otherConditional.condition as IConditionable;

                        
                        int otherCompletionTime = otherAuxConditional.GetLastCompletedConditional().GetTimeWhenWasComplete().GetTimeInNum();

                        if (completionTime > otherCompletionTime && completionTime > latestTime)
                        {
                            latestTime = completionTime;
                            lastCompleteConditional = auxConditional.GetLastCompletedConditional();
                        }
                }
           
        }
        Debug.Log("LastCompleted conditional = " + lastCompleteConditional);
        return lastCompleteConditional.GetTimeToShowNews();
    }

    public int GetChannelNum()
    {
        return channel;
    }

    int INewType.GetMinTransmitionTime()
    {
        return MinTransmitionTime;
    }

    public int GetPriority()
    {
        return priority;
    }

    public void SetWasStreamed()
    {
        wasStremed = true;
    }

    public bool GetWasStreamed()
    {
        return wasStremed;
    }

    public string GetHeadline2()
    {
        return headlineTwoLines;
    }

    
}
