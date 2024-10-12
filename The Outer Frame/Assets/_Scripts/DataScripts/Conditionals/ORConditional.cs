using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New OR Group Conditional", menuName = "Conditionals/OR Group Conditional")]
public class ORConditional : ScriptableObject, IConditionable
{
    [SerializeField] List<ConditionalClass> Conditions = new List<ConditionalClass>();

    public bool CheckIfHaveTime(){return false;}
    public bool GetAlternativeConditional(){return CheckForAllAlternativeConditionals(); }
    public bool GetStateCondition() { return CheckForAllConditionals(); }
    public TimeData GetTimeWhenWasComplete(){ throw new System.NotImplementedException();}

    public IConditionable GetLastCompletedConditional() { return GetLastCompleteConditional(); }

    bool CheckForAllConditionals()
    {
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
                conditional.SetDoit(true);
                return true;
            }
        }
        return false;
    }

    bool CheckForAllAlternativeConditionals()
    {
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
                conditional.SetDoit(true);
                return true;
            }
        }
        return false;
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

    public int GetTimeToShowNews()
    {
        throw new System.NotImplementedException();
    }
}
