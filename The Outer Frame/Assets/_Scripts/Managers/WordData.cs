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

    [Header("Ramifications")]
    [SerializeField] List<exceptions> Ramifications = new List<exceptions>();

    [Header("Inactive Conditions")]
    [SerializeField] List<ScriptableObject> InactiveConditions = new List<ScriptableObject>();

    [Header("Automatic Actions")]
    [SerializeField] List<StateEnum> AutomaticActions = new List<StateEnum>();

    [Header("Modify length actions")]
    [SerializeField] List<ModifyDurationActions> ModifyLengthactions = new List<ModifyDurationActions>();

    [Header("Reactivate Action")]
    [SerializeField] List<Re_activeActions> ReactivateAction = new List<Re_activeActions>();

    [SerializeField] WordData WordThatReplaces;
    [SerializeField] bool CopyHistory;

    private List<StateEnum> stateHistory = new List<StateEnum>();
    private List<StateEnum> CheckedStateHistory = new List<StateEnum>();
    private Dictionary<StateEnum, TimeData> StateHistoryTime = new Dictionary<StateEnum, TimeData>();
    StateEnum currentState;
    [NonSerialized] bool isFound;
    [NonSerialized] bool isPhoneNumberFound;

    private void OnEnable()
    {
        foreach (exceptions e in Ramifications) e.SetUp();
    }

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

    public int GetModifyActionDuration(StateEnum _action)
    {
        foreach(ModifyDurationActions Action in ModifyLengthactions)
        {
            if (Action.action == _action) return _action.GetTime() + Action.TimeToAdd;

        }

        return _action.GetTime();
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
    public void ChangeState(StateEnum newState)
    {
        if (CheckIfStateWasDone(newState))
        {
            Debug.LogWarning("The state " + newState.name + " was done");
            return;
        }

        exceptions exception = GetExceptions(newState);

        if (exception != null)
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
    public exceptions GetExceptions(StateEnum state)
    {
        return FindException(state);
    }

    exceptions FindException(StateEnum state)
    {
        exceptions aux = null;

        foreach (exceptions ex in Ramifications)
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

    public bool CheckIfStateAreAutomaticAction(StateEnum state)
    {
        return AutomaticActions.Contains(state);
    }

    public string GetPhoneNumber() { return PhoneNumber; }
    public bool GetIsPhoneNumberFound() { return isPhoneNumberFound; }
    public bool SetIsPhoneNumberFound() => isPhoneNumberFound = true;
    public bool GetIsAPhoneNumber() { return isAPhoneNumber; }

    public WordData GetWordThatReplaces() { return WordThatReplaces; }
    public bool GetCopyHistory() { return CopyHistory; }
    public List<StateEnum> GetHistorySeen() { return CheckedStateHistory; }
    public List<StateEnum> GetHistory() { return stateHistory; }
    
    public void CleanHistory()
    {
        stateHistory.Clear();
        CheckedStateHistory.Clear();
    }

    public void ReplaceHistory(WordData oldword)
    {
        stateHistory.Clear();
        CheckedStateHistory.Clear();

        foreach( StateEnum state in oldword.GetHistory())
        {
            stateHistory.Add(state);
        }
        foreach (StateEnum state in oldword.GetHistorySeen())
        {
            CheckedStateHistory.Add(state);
        }
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

    public void CheckForReActiveActions()
    {
        foreach(Re_activeActions reAA in ReactivateAction)
        {
            if (reAA.CheckForConditionals())
            {
                if (reAA.GetWasDone()) return;
                CleanStateFromHistory(reAA.ActionToReactivate);
                reAA.SetWasDone();
            }
        }
    }

}
[Serializable]
public class ModifyDurationActions
{
    public StateEnum action;
    public int TimeToAdd;

}



[Serializable]
public class exceptions
{
    [HideInInspector]
    public string name;
    public StateEnum Action;
    public StateEnum State;

    public bool AlsoSetDefaultState;
    public bool isOrderMatters;

    public exceptions()
    {
        name = "Name Update In Play";
        Action = null;
        State = null;
        AlsoSetDefaultState = false;
        isOrderMatters = false;

    }

    public void SetUp() {if (State != null) name = State.name;}

    public StateEnum GetState() { return CheckExceptions(); }
    public StateEnum GetStateDefault() { return Action; }

    public bool GetAlsoSetDefaultState() { return AlsoSetDefaultState; }

    [SerializeField] List<ConditionalClass> Conditions = new List<ConditionalClass>();



    public StateEnum CheckExceptions()
    {
        StateEnum auxState = State;

        if (Conditions == null) return Action;

        foreach (ConditionalClass conditional in Conditions)
        {
            IConditionable auxInterface = conditional.condition as IConditionable;

            bool conditionState = auxInterface.GetStateCondition();

            if (!conditional.ifNot)
            {
                conditionState = !conditionState;
            }

            if (conditionState)
            {
                return Action;
            }
        }

        if (isOrderMatters) return CheckIfConditionalAreInOrder();
        else
        {
            return State;
        }
    }

    StateEnum CheckIfConditionalAreInOrder()
    {
        List<int> nums = new List<int>();

        foreach (ConditionalClass conditional in Conditions)
        {
            IConditionable auxInterface = conditional.condition as IConditionable;

            if (auxInterface.CheckIfHaveTime())
            {
                nums.Add(auxInterface.GetTimeWhenWasComplete().GetTimeInNum());
            }

        }

        for (int i = 0; i < nums.Count - 1; i++)
        {
            if (nums[i] > nums[i + 1])
            {
                return Action;
            }
        }

        return State;
    }
}

[Serializable]
public class ConditionalClass
{
    public ScriptableObject condition;
    public bool ifNot;
}

[Serializable]
public class Re_activeActions
{
    public StateEnum ActionToReactivate;
    public List<ScriptableObject> Conditionals = new List<ScriptableObject>();
    public bool isOrderMatters;
    [NonSerialized] bool wasDone;

    public bool GetWasDone() { return wasDone; }

    public void SetWasDone() { wasDone = true; }

    public bool CheckForConditionals()
    {

        foreach (ScriptableObject conditional in Conditionals)
        {
            if (conditional is not IConditionable)
            {
                Debug.LogWarning(conditional.name + " is not a valid conditional");
                return false;
            }

            IConditionable auxConditional = conditional as IConditionable;

            if (!auxConditional.GetStateCondition())
            {
                return false;
            }
        }

        if (isOrderMatters) return CheckIfConditionalAreInOrder();
        else return true;
    }

    bool CheckIfConditionalAreInOrder()
    {
        List<int> nums = new List<int>();

        foreach (ScriptableObject conditional in Conditionals)
        {
            IConditionable auxConditional = conditional as IConditionable;

            if (auxConditional.CheckIfHaveTime())
            {
                nums.Add(auxConditional.GetTimeWhenWasComplete().GetTimeInNum());
            }

        }

        for (int i = 0; i < nums.Count - 1; i++)
        {
            if (nums[i] > nums[i + 1])
            {
                return false;
            }
        }

        return true;
    }
}

