using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Searched in DB", menuName = "Conditionals/SearchedInDBConditional")]
public class SearchedInDBConditional : ScriptableObject, IConditionable
{

    [SerializeField] DataBaseType DataBaseEntry;

    public bool CheckIfHaveTime(){return false;}

    
    public IConditionable GetLastCompletedConditional(){return this; }

    public bool GetStateCondition(int NemAlternativeCondition = 1){
        if (!DataBaseEntry) Debug.LogWarning("The conditional " + name + " dont have assigned a DataBaseType");
        return DataBaseEntry.GetWasSearched(); }

    public int GetTimeToShowNews(){throw new System.NotImplementedException();}

    public TimeData GetTimeWhenWasComplete(){throw new System.NotImplementedException();}
}
