using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "One Vilify Conditional", menuName = "Conditionals/ One Vilify Conditional")]
public class OneVilifyWasSendedConditional : DataType, IConditionable, IReseteableScriptableObject 
{
    [NonSerialized] bool OneVilifyWasSend;
    
    public void SetConidionTrue()
    {
        OneVilifyWasSend = true;
        MarkDirty();
    }

    public bool CheckIfHaveTime()
    {
        return false;
    }

    public IConditionable GetLastCompletedConditional()
    {
        throw new NotImplementedException();
    }

    public bool GetStateCondition(int NumOfAlternativeConditional = 1)
    {
        return OneVilifyWasSend;
    }

    public int GetTimeToShowNews()
    {
        throw new NotImplementedException();
    }

    public TimeData GetTimeWhenWasComplete()
    {
        throw new NotImplementedException();
    }

    public override void ResetScriptableObject()
    {
        OneVilifyWasSend = false;
    }
}
