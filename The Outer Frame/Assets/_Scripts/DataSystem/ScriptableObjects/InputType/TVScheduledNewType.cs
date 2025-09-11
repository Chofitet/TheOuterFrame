using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New ScheduledNew", menuName = "News/ScheduledNew")]
public class TVScheduledNewType : ScriptableObject, INewType, IReseteableScriptableObject
{
    [TextArea(minLines: 3, maxLines: 10)] [SerializeField] string headline;
    [TextArea(minLines: 3, maxLines: 10)] [SerializeField] string headlineTwoLines;
    [SerializeField] [TextArea(minLines: 3, maxLines: 10)] string text;
    [SerializeField] Sprite image;
    [SerializeField] int channel;
    [SerializeField] int Priority = 1;
    [SerializeField] int alertLevelIncrement;
    [SerializeField] bool Emergency;
    [HideInInspector][SerializeField] int Day;
    [SerializeField] int Hour;
    [SerializeField] int Minute;
    [SerializeField] int MinTransmitionTime = 10;
    [SerializeField] List<ConditionalClass> Conditions= new List<ConditionalClass>();

    [SerializeField] List<ConditionalClass> ReplacedIf = new List<ConditionalClass>();
    [SerializeField] bool isOrderMatters;

    [SerializeField] TVScheduledNewType ReplacedBy;
    [NonSerialized] bool wasStremed;

    private void OnEnable()
    {
        ScriptableObjectResetter.instance?.RegisterScriptableObject(this);
    }

    public void ResetScriptableObject()
    {
        wasStremed = false;
    }

    //Lista de condicionantes y chequeo de si son true todas para desactivar o reprogramar noticia

    public TimeData GetTimeToShow(){ return new TimeData(Day, Hour, Minute); }
    public string GetHeadline() { return headline; }

    public bool GetIfIsAEmergency() { return Emergency; }

    public int GetMinTransmitionTime() { return MinTransmitionTime; }

    Sprite INewType.GetNewImag(){ return image;}

    public TVScheduledNewType GetNew()
    {
        if (CheckForConditionals(ReplacedIf))
        {
            if (!ReplacedBy) return this;
            return ReplacedBy;
        }
        else return this;
    }

    public bool CheckForConditionals(List<ConditionalClass> list)
    {
        if (list.Count == 0) return true;
        foreach (ConditionalClass conditional in list)
        {
            if (conditional.condition is not IConditionable)
            {
                Debug.LogWarning(conditional.condition + " is not a valid conditional");
                return false;
            }
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

        if (isOrderMatters) return CheckIfConditionalAreInOrder(list);
        else return true;
    }

    bool CheckIfConditionalAreInOrder(List<ConditionalClass> list)
    {
        List<int> nums = new List<int>();

        foreach (ConditionalClass conditional in list)
        {
            IConditionable auxConditional = conditional.condition as IConditionable;

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
        TimeData timeNew = GetTimeToShow();
        TimeData ActualTime = TimeManager.timeManager.GetTime();

        if (timeNew.Day == ActualTime.Day && timeNew.Hour == ActualTime.Hour && timeNew.Minute == (ActualTime.Minute))
        {
            if(CheckForConditionals(Conditions))
            {
                return true;
            }
        }
        return false;
    }

    public int GetTimeToAppear()
    {
        return 0;
    }

    public int GetChannelNum()
    {
        return channel;
    }

    public int GetPriority()
    {
        return Priority;
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
