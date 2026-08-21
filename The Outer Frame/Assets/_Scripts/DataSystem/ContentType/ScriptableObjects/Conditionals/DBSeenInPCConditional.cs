using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New DB Seen In PC Conditional", menuName = "Conditionals/DBSeenInPCConditional")]
public class DBSeenInPCConditional : ScriptableObject, IConditionable
{

    [SerializeField] DataBaseType DataBaseEntry;
    public bool CheckIfHaveTime()
    {
        return false;
    }

    public IConditionable GetLastCompletedConditional()
    {
        return this;
    }

    public bool GetStateCondition(int NumOfAlternativeConditional = 1)
    {
        return DataBaseEntry.GetWasSeen();
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
