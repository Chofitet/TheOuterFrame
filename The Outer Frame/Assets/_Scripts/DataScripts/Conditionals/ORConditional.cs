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

            if (!conditional.ifNot)
            {
                conditionState = !conditionState;
            }

            if (conditionState)
            {
                return true;
            }
        }
        return false;
    }

}
