using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New State", menuName = "State")]
public class StateEnum : ScriptableObject
{
    [SerializeField] string action;
    [SerializeField] int TimeToComplete;
    [SerializeField] int TimeToShowNew;
    [NonSerialized] bool isInactive;

    private void Awake()
    {
        isInactive = false;
    }

    public int GetTime() { return TimeToComplete; }

    public int GetTimeToShowNew() { return TimeToShowNew; }

    public string GetActionVerb() { return action; }

    public void SetActiveOrDesactive(bool x) { 
        isInactive = x; 
    }

    public bool GetIfIsActive() {
        
        
        return isInactive == true ? false : true; }
}
