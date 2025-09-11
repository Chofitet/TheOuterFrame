using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "New Goal",menuName ="Goal")]
public class BoardGoals : ScriptableObject
{
    [SerializeField] string Description;
    [SerializeField] List<ScriptableObject> ConditionalsToAdd = new List<ScriptableObject>();
    [SerializeField] bool isOrderMatters1;
    [SerializeField] List<ScriptableObject> ConditionalsToComplete = new List<ScriptableObject>();
    [SerializeField] bool isOrderMatters2;


    public string GetDescription() { return Description; }

    public bool GetIsConditionalsToAdd() { return CheckForConditionals(ConditionalsToAdd, isOrderMatters1); }
    public bool GetIsConditionalsToComplete() { return CheckForConditionals(ConditionalsToComplete, isOrderMatters2); }

    public bool CheckForConditionals(List<ScriptableObject> list, bool isOrderMatters)
    {
        
        foreach (ScriptableObject conditional in list)
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

        if (isOrderMatters) return CheckIfConditionalAreInOrder(list);
        else return true;
    }

    bool CheckIfConditionalAreInOrder(List<ScriptableObject> list)
    {
        List<int> nums = new List<int>();

        foreach (ScriptableObject conditional in list)
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
