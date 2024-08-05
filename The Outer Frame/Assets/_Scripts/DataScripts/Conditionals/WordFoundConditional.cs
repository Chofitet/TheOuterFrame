using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Word Found Conditional", menuName = "Conditionals/WordFoundConditional")]
public class WordFoundConditional : ScriptableObject, IConditionable
{
    [SerializeField] WordData word;

    public bool CheckIfHaveTime()
    {
        return false;
    }

    public bool GetStateCondition()
    {
        return word.GetIsFound();
    }

    public TimeData GetTimeWhenWasComplete()
    {
        throw new System.NotImplementedException();
    }
}
