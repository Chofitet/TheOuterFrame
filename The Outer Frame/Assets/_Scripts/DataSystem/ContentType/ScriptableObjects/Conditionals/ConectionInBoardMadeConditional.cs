using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Connection Made Conditional", menuName = "Conditionals/ConnectionMadeConditional")]
public class ConectionInBoardMadeConditional : IConditionable
{

    public bool CheckIfHaveTime()
    {
        return false;
    }

    public IConditionable GetLastCompletedConditional()
    {
        throw new System.NotImplementedException();
    }

    public bool GetStateCondition(int NumOfAlternativeConditional)
    {
        return true;
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
