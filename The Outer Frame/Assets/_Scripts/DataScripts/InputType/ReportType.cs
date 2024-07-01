using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Report", menuName = "Input/Report")]
public class ReportType : ScriptableObject, IStateComparable
{
    [SerializeField] StateEnum state;
    [SerializeField] string Text;

    public StateEnum GetState() { return state;}

    public string GetText() {  return Text;}

    private TimeData CompleteTime;
    public void SetTimeWhenWasDone()
    {
        CompleteTime = TimeManager.timeManager.GetTime();
    }

    public TimeData GetTimeWhenWasDone() { return CompleteTime; }

    


}