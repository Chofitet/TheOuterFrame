using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New TVNew", menuName = "Input/TVNew")]
public class TVNewType : ScriptableObject, IStateComparable
{
    [SerializeField] StateEnum state;
    [SerializeField] string headline;
    [SerializeField] Sprite image;
    [SerializeField] int channel;
    [SerializeField] int priority;
    [SerializeField] int alertLevelIncrement;


    public StateEnum GetState()
    {
        return state;
    }

    private TimeData CompleteTime;
    public void SetTimeWhenWasDone()
    {
        CompleteTime = TimeManager.timeManager.GetTime();
    }

    public TimeData GetTimeWhenWasDone() { return CompleteTime;}

}
