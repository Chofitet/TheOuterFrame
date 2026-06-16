using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New AND Group Conditional", menuName = "Conditionals/AND Group Conditional")]
public class ANDConditional : ScriptableObject, IConditionable
{
    [SerializeField] List<ConditionalClass> Conditions = new List<ConditionalClass>();

    public bool CheckIfHaveTime()
    {
        return false;
    }

    public IConditionable GetLastCompletedConditional() { return GetLastCompleteConditional(); }

    public bool GetStateCondition(int NumAlternativeCondition = 1)
    {
         return CheckForAllConditionals(NumAlternativeCondition);
    }

    public int GetTimeToShowNews()
    {
        throw new System.NotImplementedException();
    }

    public TimeData GetTimeWhenWasComplete()
    {
        throw new System.NotImplementedException();
    }

    bool CheckForAllConditionals(int NumAlternativeCondition)
    {
        foreach (ConditionalClass conditional in Conditions)
        {
            IConditionable auxInterface = conditional.condition as IConditionable;

            if (auxInterface == null)
                throw new Exception("La condición no implementa IConditionable.");

            bool conditionState = auxInterface.GetStateCondition(NumAlternativeCondition);

            if (conditional.ifNot)
            {
                conditionState = !conditionState;
            }

            if (!conditionState)
            {
                return false;
            }
        }
        return true;
    }


    IConditionable GetLastCompleteConditional()
    {
        IConditionable lastCompleteConditional = null;
        int latestTime = 0;

        foreach (ConditionalClass conditional in Conditions)
        {
            IConditionable auxConditional = conditional.condition as IConditionable;

            if (auxConditional.CheckIfHaveTime())
            {
                int completionTime = auxConditional.GetTimeWhenWasComplete().GetTimeInNum();

                foreach (ConditionalClass otherConditional in Conditions)
                {
                    IConditionable otherAuxConditional = otherConditional.condition as IConditionable;

                    if (otherAuxConditional.CheckIfHaveTime())
                    {
                        int otherCompletionTime = otherAuxConditional.GetTimeWhenWasComplete().GetTimeInNum();

                        if (completionTime > otherCompletionTime && completionTime > latestTime)
                        {
                            latestTime = completionTime;
                            lastCompleteConditional = auxConditional;
                        }
                    }
                }
            }
        }
        Debug.Log("LastCompleted conditional = " + lastCompleteConditional);
        return lastCompleteConditional;
    }

}
