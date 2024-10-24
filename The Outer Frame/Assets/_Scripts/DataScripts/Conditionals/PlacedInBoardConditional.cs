using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Placed In Board Conditional", menuName = "Conditionals/PlacedInBoardConditional")]
public class PlacedInBoardConditional : ScriptableObject, IConditionable
{
    [SerializeField] WordData Word;

    public bool CheckIfHaveTime()
    {
        return false;
    }


    public IConditionable GetLastCompletedConditional()
    {
        return this as IConditionable;
    }

    public bool GetStateCondition(int NumOfAlternative = 1)
    {
        return Word.GetPlacedInBoard();
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
