using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ScheduledNew", menuName = "News/ScheduledNew")]
public class TVScheduledNewType : ScriptableObject, INewType
{
    [TextArea(minLines: 3, maxLines: 10)] [SerializeField] string headline;
    [SerializeField] Sprite image;
    [SerializeField] int channel;
    [SerializeField] int alertLevelIncrement;
    [SerializeField] bool Emergency;
    [SerializeField] int Day;
    [SerializeField] int Hour;
    [SerializeField] int Minute;

    [SerializeField] List<ScriptableObject> ReplacedIf = new List<ScriptableObject>();
    [SerializeField] bool isOrderMatters;

    [SerializeField] TVScheduledNewType NewToReplace;

    //Lista de condicionantes y chequeo de si son true todas para desactivar o reprogramar noticia

    public TimeData GetTimeToShow(){ return new TimeData(Day, Hour, Minute); }
    public string GetHeadline() { return headline; }

    public bool GetIfIsAEmergency() { return Emergency; }

    Sprite INewType.GetNewImag(){ return image;}

    public TVScheduledNewType GetNew()
    {
        if (CheckForConditionals())
        {
            if (!NewToReplace) return this;
            return NewToReplace;
        }
        else return this;
    }

    public bool CheckForConditionals()
    {

        foreach (ScriptableObject conditional in ReplacedIf)
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

        foreach (ScriptableObject conditional in ReplacedIf)
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
