using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Goo Protocol Conditional", menuName = "Conditionals/GooProtocolConditional")]
public class GooProtocolCompleteConditional : DataType, IConditionable, IReseteableScriptableObject
{


    [NonSerialized] bool conditionalState = false;

    public override void ResetScriptableObject()
    {
        conditionalState = false;
    }

    public void SetConditionalState()
    {
        conditionalState = true;
        MarkDirty();
    }

    public bool CheckIfHaveTime()
    {
        return false;
    }

    public IConditionable GetLastCompletedConditional()
    {
        throw new System.NotImplementedException();
    }

    public bool GetStateCondition(int NumOfAlternativeConditional = 1)
    {
        return conditionalState;
    }

    public int GetTimeToShowNews()
    {
        throw new System.NotImplementedException();
    }

    public TimeData GetTimeWhenWasComplete()
    {
        throw new System.NotImplementedException();
    }

   
}
