using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New BDEnter", menuName = "Input/BDEnter")]
public class DataBaseType : ScriptableObject, IStateComparable
{
    [SerializeField] StateEnum state;
    [SerializeField] string text;
    [SerializeField] Sprite image;
    [SerializeField] string phoneNum;

    

    public StateEnum GetState() { return state; }
    
    public string GetText() { return text; }

    private TimeData CompleteTime;
    public void SetTimeWhenWasDone()
    {
        CompleteTime = TimeManager.timeManager.GetTime();
    }

    public TimeData GetTimeWhenWasDone() { return CompleteTime; }



}
