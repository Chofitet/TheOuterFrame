using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ScheduledNew", menuName = "News/ScheduledNew")]
public class TVScheduledNewType : ScriptableObject, INewType
{
    [TextArea(minLines: 3, maxLines: 10)] [SerializeField] string headline;
    [SerializeField] [TextArea(minLines: 3, maxLines: 10)] string text;
    [SerializeField] Sprite image;
    [SerializeField] int channel;
    [SerializeField] int alertLevelIncrement;
    [SerializeField] bool Emergency;
    [SerializeField] int Day;
    [SerializeField] int Hour;
    [SerializeField] int Minute;

    [SerializeField] List<ConditionalClass> ReplacedIf = new List<ConditionalClass>();
    [SerializeField] bool isOrderMatters;

    [SerializeField] TVScheduledNewType ReplacedBy;

    //Lista de condicionantes y chequeo de si son true todas para desactivar o reprogramar noticia

    public TimeData GetTimeToShow(){ return new TimeData(Day, Hour, Minute); }
    public string GetHeadline() { return headline; }

    public bool GetIfIsAEmergency() { return Emergency; }

    Sprite INewType.GetNewImag(){ return image;}

    public TVScheduledNewType GetNew()
    {
        if (CheckForConditionals())
        {
            if (!ReplacedBy) return this;
            return ReplacedBy;
        }
        else return this;
    }

    public bool CheckForConditionals()
    {

        foreach (ConditionalClass conditional in ReplacedIf)
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

        if (isOrderMatters) return CheckIfConditionalAreInOrder();
        else return true;
    }

    bool CheckIfConditionalAreInOrder()
    {
        List<int> nums = new List<int>();

        foreach (ConditionalClass conditional in ReplacedIf)
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
}
