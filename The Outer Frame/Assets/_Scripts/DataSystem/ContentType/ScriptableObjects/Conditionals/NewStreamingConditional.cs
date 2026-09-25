using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "was broadcasted conditional", menuName = "Conditionals/Was broadcasted conditional")]
public class NewStreamingConditional : ScriptableObject, IConditionable
{
    [SerializeField] ScriptableObject New;

    bool wasStremed;
    public bool CheckIfHaveTime()
    {
        return false;
    }

    

    public IConditionable GetLastCompletedConditional()
    {
        throw new System.NotImplementedException();
    }

    public bool GetStateCondition(int NumOfAlternativeCondition = 1)
    {
        if (New is not INewType) Debug.LogWarning("The conditional " + name + " have a wrong type of Scriptable Object assigned in a field");
        INewType _new = New as INewType;

        wasStremed = _new.GetWasStreamed();

        return _new.GetWasStreamed();
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
