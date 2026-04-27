using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "One Vilify Conditional", menuName = "Conditionals/ One Vilify Conditional")]
public class OneVilifyWasSendedConditional : ScriptableObject, IConditionable, IReseteableScriptableObject 
{
    [NonSerialized] bool OneVilifyWasSend;
    private void OnEnable()
    {
        ScriptableObjectResetter.instance?.RegisterScriptableObject(this);
    }

    public void SetConidionTrue()
    {
        OneVilifyWasSend = true;
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

    public void ResetScriptableObject()
    {
        OneVilifyWasSend = false;
    }
}
