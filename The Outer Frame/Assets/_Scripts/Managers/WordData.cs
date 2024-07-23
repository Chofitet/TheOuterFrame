using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Word", menuName ="Word")]
public class WordData : ScriptableObject
{
    [Header("Word General Data")]
    [SerializeField] string wordName;
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

    [Header("Exceptions")]
    [SerializeField] List<Exceptions> exceptions = new List<Exceptions>();

    [Header("Inactive Conditions")]
    [SerializeField] List<ScriptableObject> InactiveConditions = new List<ScriptableObject>();

    [Header("Automatic Actions")]
    [SerializeField] List<StateEnum> AutomaticActions = new List<StateEnum>();

    [SerializeField] WordData WordThatReplaces;

    private List<StateEnum> stateHistory = new List<StateEnum>();
    private List<StateEnum> CheckedStateHistory = new List<StateEnum>();
    private Dictionary<StateEnum, TimeData> StateHistoryTime = new Dictionary<StateEnum, TimeData>();
    StateEnum currentState;
    [NonSerialized] bool isFound;
    [NonSerialized] bool isPhoneNumberFound;

    #region GetInputLogic


    public TVNewType GetTVnew(StateEnum state)
    {
        return FindInputInList(TVNewTypes, state);
    }

    public ReportType GetReport(StateEnum state, bool isSetTime = false)
    {
        ReportType input = FindInputInList(reportTypes, state);
        if(isSetTime && input != default) input.GetTimeWhenWasDone();
        return input;
    }

    public ReportType GetLastReport()
    {
        return GetReport(currentState, true);
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

    T  FindInputInList<T>(List<T> list, StateEnum state) where T : IStateComparable
    {
        T aux = default;

        foreach(T input in list)
        {
           if (state == input.GetState())
           {
                aux = input;
           }
        }
        return aux;
    }

    #endregion

    #region ChangeStateLogic
    public void ChangeState(StateEnum newState)
    {

        if (CheckIfStateWasDone(newState))
        {
            Debug.LogWarning("The state " + newState.name + " was done");
            return;
        }

        Exceptions exception = GetExceptions(newState);

        if (exception)
        {
            currentState = exception.GetState();

            if (currentState != newState)
            {
                if (exception.GetAlsoSetDefaultState())
                {
                    stateHistory.Add(newState);
                    StateHistoryTime.Add(newState, TimeManager.timeManager.GetTime());
                }
            }
        }
        else
        {
            currentState = newState;
        }

        stateHistory.Add(currentState);
        StateHistoryTime.Add(currentState, TimeManager.timeManager.GetTime());


        string estados = wordName + ": ";
        foreach (StateEnum s in stateHistory)
        {

            estados += s.name + ", ";
        }

        Debug.Log(estados);

    }
    public Exceptions GetExceptions(StateEnum state)
    {
        return FindException(state);
    }

    Exceptions FindException(StateEnum state)
    {
        Exceptions aux = null;

        foreach (Exceptions ex in exceptions)
        {
            if (state == ex.GetStateDefault())
            {
                if (state != ex.GetState())
                {
                    aux = ex;
                }
            }
        }

        return aux;
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
        if (CheckIfStateSeenWasDone(newState))
        {
            Debug.LogWarning("The state " + newState.name + " was seen");
            return;
        }
        CheckedStateHistory.Add(newState);
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

    public bool CheckIfStateAreAutomaticAction(StateEnum state)
    {
        return AutomaticActions.Contains(state);
    }

    public string GetPhoneNumber() { return PhoneNumber; }
    public bool GetIsPhoneNumberFound() { return isPhoneNumberFound; }
    public bool SetIsPhoneNumberFound() => isPhoneNumberFound = true;
    public bool GetIsAPhoneNumber() { return isAPhoneNumber; }

    public WordData GetWordThatReplaces() { return WordThatReplaces; }
    public List<StateEnum> GetHistorySeen() { return CheckedStateHistory; }
    public List<StateEnum> GetHistory() { return stateHistory; }
    
    public void CleanHistory()
    {
        stateHistory.Clear();
        CheckedStateHistory.Clear();
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

    public void SetIsFound(bool x = true) => isFound = x;

    public bool GetIsFound() { return isFound; }

}
