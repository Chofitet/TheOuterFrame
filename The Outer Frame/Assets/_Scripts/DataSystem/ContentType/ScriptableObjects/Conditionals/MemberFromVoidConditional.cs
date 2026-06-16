using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Member From Void Conditional", menuName = "Conditionals/Member From Void Conditional")]
public class MemberFromVoidConditional : ScriptableObject, IConditionable
{
    [SerializeField] WordData MemberWord;
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
        return MemberWord.GetWordWasRemember();
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
