using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InfoPostItController : MonoBehaviour, IPlacedOnBoard
{
    [SerializeField] List<StringConnectionController> StringConnections;
    [SerializeField] List<ScriptableObject> Conditionals = new List<ScriptableObject>();
    [SerializeField] List<bool> ifNot = new List<bool>();
    [SerializeField] bool isOrderMatters;
    
    bool isActiveInBegining;

    private void Start()
    {
        if(Conditionals.Count == 0 && StringConnections.Count == 0)
        {
            isActiveInBegining = true;
        }
    }

    public bool GetConditionalState()
    {
        if (StringConnections.Count != 0)
        {
            foreach (StringConnectionController connection in StringConnections)
            {
                if(!connection.GetIsConnected()) return false;
            }
        }

        return CheckForConditionals();
    }

    public bool CheckForConditionals()
    {
        int index = 0;
        foreach (ScriptableObject conditional in Conditionals)
        {
            bool ifnot = false;
            if(ifNot.Count != 0) ifnot = ifNot[index];
            index++;
            if (conditional is not IConditionable)
            {
                Debug.LogWarning(conditional.name + " is not a valid conditional");
                return false;
            }

            IConditionable auxConditional = conditional as IConditionable;

            bool conditionState = auxConditional.GetAlternativeConditional();

            if (!ifnot)
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

