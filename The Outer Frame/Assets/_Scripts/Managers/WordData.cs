using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Word", menuName ="Word")]
public class WordData : ScriptableObject
{
    [Header("Word General Data")]
    [SerializeField] string wordName;
    [SerializeField] List<string> FindableAs = new List<string>();
    [SerializeField] string Form_DatabaseNameVersion;
    [SerializeField] string ProgressorNameVersion;
    [SerializeField] string PhoneNumber;
    [SerializeField] bool isAPhoneNumber;

    [Header("Reports")]
    [SerializeField] List<ReportType> reportTypes = new List<ReportType>();

    [Header("TV News")]
    [SerializeField] List<TVNewType> TVNewTypes = new List<TVNewType>();

    [Header("BD Data")]
    [SerializeField] DataBaseType DBTypes;

    [Header("Calls")]
    [SerializeField] List<CallType> CallTypes = new List<CallType>();
    
    /*[Header("Availability call Window:")]
    [SerializeField] TimeCheckConditional StartTime;
    [SerializeField] TimeCheckConditional EndTime;*/

    [Header("Inactive Conditions")]
    [SerializeField] List<ScriptableObject> InactiveConditions = new List<ScriptableObject>();

    [SerializeField] WordData WordThatReplaces;
    [SerializeField] bool CopyHistory;

    [NonSerialized] List<StateEnum> stateHistory = new List<StateEnum>();
    [NonSerialized] List<StateEnum> CheckedStateHistory = new List<StateEnum>();
    [NonSerialized] Dictionary<StateEnum, TimeData> StateHistoryTime = new Dictionary<StateEnum, TimeData>();
    StateEnum currentState;
    private List<ActionState> ActionsStates = new List<ActionState>();
    [NonSerialized] bool isFound;
    [NonSerialized] bool isPhoneNumberFound;

    #region GetInputLogic


    public TVNewType GetTVnew(StateEnum state)
    {
        return FindInputInList(TVNewTypes, state);
    }

    public ReportType GetReport(StateEnum state)
    {
        if (!state) return null;
        //filtrado por acción
        List<ReportType> ListFilteredByState = new List<ReportType>();

        foreach (ReportType report in reportTypes)
        {
            if (!report) continue;
            if (report.GetAction() == null) continue;
            if(report.GetAction() == state)
            {
                ListFilteredByState.Add(report);
            }
        }

        //Filtrado por cumplimiento de condicional
        foreach (ReportType report in ListFilteredByState)
        {
            if(report.CheckConditionals())
            {
                if(report.CheckIfIsDefault() && GetLastReportOfAnAction(state) != null) continue;

                return report;
            }
        }

        //Devuelve el reporte guardado por último para esa acción para que luego se muestre como acción repetida
        return GetLastReportOfAnAction(state);

    }

    public ReportType GetReportFromState(StateEnum state)
    {
        foreach (ReportType report in reportTypes)
        {
            if (!report) continue;
            if (report.GetState() == state) return report;
        }
        return null;
    }
    ReportType GetLastReportOfAnAction(StateEnum state)
    {

        ReportType aux = null;
        foreach(ActionState AS in ActionsStates)
        {
            if (AS.GetState() == state) aux = AS.GetLastReport();
        }

        return aux;
    }


    public DataBaseType GetDB()
    {
        return DBTypes;
    }

    public List<CallType> GetCall()
    {
        return CheckForCallInTimeZone();
    }

    public void SetReactionCall(StateEnum state)
    {
        CallType call = FindInputInList(CallTypes, state);
        if (call == default) return;
        call.DefineTimeZone();
    }

    private List<CallType> CheckForCallInTimeZone()
    {
        //Call from Pinchofono every minute to find a call in timeZone
        List<CallType> auxList = new List<CallType>();

        foreach (CallType call in CallTypes)
        {
            if(call.CheckForTimeZone())
            {
                auxList.Add(call);
            }
        }

        return auxList;
    }

    public List<CallType> GetAllCathedCalls()
    {
        List<CallType> auxList = new List<CallType>();

        foreach(CallType call in CallTypes)
        {
            if (call.GetIsCatch())
            {
                auxList.Add(call);
            }
        }

        return auxList;
    }

    T  FindInputInList<T>(List<T> list, StateEnum state) where T : IStateComparable
    {
        T aux = default;

        foreach(T input in list)
        {
            if (input == null) continue;
            if (state == input.GetState())
           {
                aux = input;
           }
        }
        return aux;
    }

    #endregion

    #region ChangeStateLogic
    public void ChangeState(ReportType report)
    {
        stateHistory.Add(report.GetState());
        StateHistoryTime.Add(report.GetState(), TimeManager.timeManager.GetTime());

        string estados = wordName + ": ";
        foreach (StateEnum s in stateHistory)
        {
            estados += s.name + ", ";
        }

        Debug.Log(estados);

        foreach(ActionState AS in ActionsStates)
        {
            if(AS.GetState() == report.GetAction())
            {
                AS.SetReport(report);
            }
        }

    }
    public bool CheckIfStateWasDone(StateEnum state)
    {
        for (int i = 0; i < stateHistory.Count; i++)
        {
            if (stateHistory[i] == state)
            {
                return true;
            }
        }
        return false;
    }

    public bool CheckIfStateSeenWasDone(StateEnum state)
    {
        for (int i = 0; i < CheckedStateHistory.Count; i++)
        {
            if (CheckedStateHistory[i] == state)
            {
                return true;
            }
        }
        return false;
    }
    public void CheckStateSeen(StateEnum newState)
    {
        CheckedStateHistory.Add(newState);
    }

    public void AddStateInHistory(StateEnum newState)
    {
        stateHistory.Add(newState);
    
    }

    public void CleanStateFromHistory(StateEnum state)
    {
        if(stateHistory.Contains(state))
        {
            stateHistory.Remove(state);
        }
    }

    #endregion

    #region InactiveLogic

    public bool GetInactiveState() {

        return CheckInactiveConditions();
    }

     bool CheckInactiveConditions()
    {
        if (InactiveConditions.Count == 0) return false;

        foreach (ScriptableObject conditional in InactiveConditions)
        {
            IConditionable auxInterface = conditional as IConditionable;

            if (!auxInterface.GetStateCondition())
            {
                return false;
            }
        }
        return true;
    }

    #endregion

    public string GetName() { return wordName; }

    public string FindFindableName(string wordCompere){
        string aux = "";
        foreach(string s in FindableAs)
        {
            if(s == wordCompere)
            {
                aux = s;
            }
        }
        return aux;
    }

    public string GetForm_DatabaseNameVersion() 
    {
        if (Form_DatabaseNameVersion == "") return wordName;
        else return Form_DatabaseNameVersion; 
    }
    public string GetProgressorNameVersion() 
    {
        if (ProgressorNameVersion == "") return GetForm_DatabaseNameVersion();
        else return ProgressorNameVersion; 
    }
    public string GetPhoneNumber() { return PhoneNumber; }
    public bool GetIsPhoneNumberFound() { return isPhoneNumberFound; }
    public bool SetIsPhoneNumberFound() => isPhoneNumberFound = true;
    public bool GetIsAPhoneNumber() { return isAPhoneNumber; }

    public WordData GetWordThatReplaces() { return WordThatReplaces; }
    public bool GetCopyHistory() { return CopyHistory; }
    public List<StateEnum> GetHistorySeen() { return CheckedStateHistory; }
    public List<StateEnum> GetHistory() { return stateHistory; }
    public List<ActionState> GetActionStatesList() { return ActionsStates; }
    public void SetListOfActions(List<StateEnum> actions)
    {
        ActionsStates.Clear();
        foreach (StateEnum state in actions)
        {
            
            ActionsStates.Add(new ActionState(state));
        }
    }

    public void CleanHistory()
    {
        stateHistory.Clear();
        CheckedStateHistory.Clear();
    }

    public void ReplaceHistory(WordData oldword)
    {
        stateHistory.Clear();
        CheckedStateHistory.Clear();
        ActionsStates.Clear();

        foreach ( StateEnum state in oldword.GetHistory())
        {
            stateHistory.Add(state);
        }
        foreach (StateEnum state in oldword.GetHistorySeen())
        {
            CheckedStateHistory.Add(state);
        }

        ActionsStates = oldword.GetActionStatesList();
    }

    public TimeData GetTimeOfState(StateEnum state)
    {
        foreach (StateEnum s in StateHistoryTime.Keys)
        {
            if (s == state)
            {
                return StateHistoryTime[s];
            }
        }
        return TimeManager.timeManager.GetTime();
    }

    public void SetIsFound(bool x = true)
    {
        isFound = x;
    }

    public bool GetIsFound() { return isFound; }

}
[Serializable]
public class ActionState
{
    StateEnum state;
    ReportType LastReport = null;

    public ActionState(StateEnum _state)
    {
        state = _state;
    }

    public StateEnum GetState() { return state; }
    public ReportType GetLastReport() { return LastReport; }
    public void SetReport(ReportType report){ LastReport = report; }
}
