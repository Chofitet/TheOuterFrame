using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New Minimun Completed Conditions Group", menuName = "Conditionals/Conditions Counter Conditional")]
public class MinimunConditionsCompleteConditional : ScriptableObject, IConditionable
{
    [SerializeField] int MinConditionsForTrue;
    [SerializeField] List<ConditionalClass> Conditions = new List<ConditionalClass>();
    
    public bool CheckIfHaveTime() {return false;}

    public bool GetAlternativeConditional(){ return CheckForAllAlternativeConditionals(); }

    public IConditionable GetLastCompletedConditional(){ return GetLastCompleteConditional(); }

    public bool GetStateCondition(){ return CheckForAllConditionals(); }

    public int GetTimeToShowNews(){throw new System.NotImplementedException();}

    bool CheckForAllConditionals()
    {
        int index = 0;
        foreach (ConditionalClass conditional in Conditions)
        {
            IConditionable auxInterface = conditional.condition as IConditionable;

            bool conditionState = auxInterface.GetStateCondition();

            if (conditional.ifNot)
            {
                conditionState = !conditionState;
            }

            if (conditionState)
            {
                // + if is true
                index++;
            }
        }

        if (index >= MinConditionsForTrue) return true;
        else return false;
    }

    bool CheckForAllAlternativeConditionals()
    {
        int index = 0;
        foreach (ConditionalClass conditional in Conditions)
        {
            IConditionable auxInterface = conditional.condition as IConditionable;

            bool conditionState = auxInterface.GetAlternativeConditional();

            if (conditional.ifNot)
            {
                conditionState = !conditionState;
            }

            if (conditionState)
            {
                // + if is true
                index++;
            }
        }

        if (index >= MinConditionsForTrue) return true;
        else return false;
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
        return lastCompleteConditional;
    }

    public TimeData GetTimeWhenWasComplete()
    {
        throw new System.NotImplementedException();
    }
}
