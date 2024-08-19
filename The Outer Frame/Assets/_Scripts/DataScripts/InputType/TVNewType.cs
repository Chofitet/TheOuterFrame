using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New TVNew", menuName = "News/ReactiveNew")]
public class TVNewType : ScriptableObject, IStateComparable, INewType
{
    [SerializeField] StateEnum state;
    [SerializeField] string headline;
    [SerializeField] Sprite image;
    [SerializeField] int channel;
    [SerializeField] int priority;
    [SerializeField] int alertLevelIncrement;
    [SerializeField] bool Emergency;


    public StateEnum GetState()
    {
        return state;
    }

    private TimeData CompleteTime;
    public void SetTimeWhenWasDone()
    {
        CompleteTime = TimeManager.timeManager.GetTime();
    }

    public string GetHeadline() { return headline; }
    public TimeData GetTimeWhenWasDone() { return CompleteTime;}

    public Sprite GetNewImag(){return image;}

    public bool GetIfIsAEmergency(){ return Emergency;}

    public int GetIncreaseAlertLevel()
    {
        return alertLevelIncrement;
    }
}
