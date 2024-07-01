using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New exception", menuName = "Input/Exception")]
public class Exceptions : ScriptableObject
{
    [SerializeField] StateEnum DefaultState;
    [SerializeField] StateEnum SpecialState;

    [SerializeField] bool AlsoSetDefaultState;
    [SerializeField] bool isOrderMatters;

    public StateEnum GetState() {return CheckExceptions();}
    public StateEnum GetStateDefault() { return DefaultState; }

    public bool GetAlsoSetDefaultState() { return AlsoSetDefaultState; }

    [SerializeField] List<ScriptableObject> exceptionConditions = new List<ScriptableObject>();

    public StateEnum CheckExceptions()
    {
        StateEnum auxState = SpecialState;

        if (exceptionConditions == null) return DefaultState;
        
        foreach(ScriptableObject conditional in exceptionConditions)
        {
            IConditionable auxInterface = conditional as IConditionable;

            if (!auxInterface.GetStateCondition())
            {
                return DefaultState;
            }
        }

        if (isOrderMatters) return CheckIfConditionalAreInOrder();
        else return SpecialState;
    }

     StateEnum CheckIfConditionalAreInOrder()
    {
        List<int> nums = new List<int>();

        foreach (ScriptableObject conditional in exceptionConditions)
        {
            IConditionable auxInterface = conditional as IConditionable;

            if(auxInterface.CheckIfHaveTime())
            {
                nums.Add(auxInterface.GetTimeWhenWasComplete().GetTimeInNum());
            }
            
        }

        for (int i = 0; i < nums.Count - 1; i++)
        {
            if (nums[i] > nums[i + 1])
            {
                return DefaultState;
            }
        }

        return SpecialState;
    }

}
