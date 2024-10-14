using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New State", menuName = "State")]
public class StateEnum : ScriptableObject, IReseteableScriptableObject
{
    [Header("Action Fields")]
    [SerializeField] string InfinitiveVerb;
    [SerializeField] string Actioning;
    [SerializeField] string Actioned;
    [SerializeField] int TimeToComplete;
    [SerializeField] int TimeToShowNew;
    [SerializeField] string observationtxt = "";
    [SerializeField] Agent Agent;
    [Header("IdeaInProgressor")]
    [SerializeField] WordData IdeaWordData;
    [SerializeField] string IdeaVerb;
    [SerializeField] string IdeaWord;
    [NonSerialized] bool isDone;

    private void OnEnable()
    {
        ScriptableObjectResetter.instance?.RegisterScriptableObject(this);
    }

    public void ResetScriptableObject()
    {
        isDone = false;
    }

    public int GetTime() { return TimeToComplete; }
    public int GetTimeToShowNew() { return TimeToShowNew; }
    public string GetInfinitiveVerb() { return InfinitiveVerb; }
    public string GetActioningVerb() { return Actioning; }
    public string GetActionedVerb() { return Actioned; }

    public WordData GetSpecialActionWord() { return IdeaWordData; }

    public string GetSpeticialActionWordName()
    {
        if (IdeaWord == "") return IdeaWordData.GetProgressorNameVersion();
        else return IdeaWord;
    }

    public string GetObservationTxt() { return observationtxt; }

    public string GetIdeaVerb() { return IdeaVerb; }

    public void SetIsDone(bool x) => isDone = x;

    public bool GetIsDone() { return isDone; }

    public void SetActiveOrDesactiveAgent(bool x) {
       // Agent.SetActiveDesactive(x); 
    }

    public bool GetIfIsActive() {
        /* if (!Agent)
         {
             Debug.LogWarning(Actioning + " dont have Agent Assigned");
             return false;
         }

         return Agent.GetIsActive(); */
        return true;
    }
}
