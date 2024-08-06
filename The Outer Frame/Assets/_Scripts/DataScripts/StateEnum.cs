using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New State", menuName = "State")]
public class StateEnum : ScriptableObject
{
    [Header("Action Fields")]
    [SerializeField] string ActionVerb;
    [SerializeField] int TimeToComplete;
    [SerializeField] int TimeToShowNew;
    [SerializeField] Agent Agent;
    [SerializeField] WordData SpecialActionWord;
    [SerializeField] string observationtxt = "";

    public int GetTime() { return TimeToComplete; }

    public int GetTimeToShowNew() { return TimeToShowNew; }

    public string GetActionVerb() { return ActionVerb; }

    public WordData GetSpecialActionWord() { return SpecialActionWord; }

    public string GetObservationTxt() { return observationtxt; }

    public void SetActiveOrDesactiveAgent(bool x) {
        Agent.SetActiveDesactive(x); 
    }

    public bool GetIfIsActive() {
        if (!Agent)
        {
            Debug.LogWarning(ActionVerb + " dont have Agent Assigned");
            return false;
        }
            
        return Agent.GetIsActive(); 
    }
}
