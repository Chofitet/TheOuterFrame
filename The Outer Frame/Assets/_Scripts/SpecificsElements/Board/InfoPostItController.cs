using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InfoPostItController : MonoBehaviour, IPlacedOnBoard
{
    [SerializeField] List<ScriptableObject> Conditionals = new List<ScriptableObject>();
    [SerializeField] bool isOrderMatters;

    bool isActiveInBegining;

    private void Start()
    {
        if(Conditionals.Count == 0)
        {
            isActiveInBegining = true;
        }
    }

    public bool GetConditionalState()
    {
        return CheckForConditionals();
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

            if (!auxConditional.GetAlternativeConditional())
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

    public bool ActiveInBegining()
    {
        return isActiveInBegining;
    }

    public bool GetIsTaken()
    {
        return false;
    }
}

