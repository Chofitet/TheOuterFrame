using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

[CreateAssetMenu(fileName = "New Word", menuName ="Word")]
public class WordData : ScriptableObject, IReseteableScriptableObject
{
    [Header("Word General Data")]
    [SerializeField] string wordName;
    [SerializeField] List<string> FindableAs = new List<string>();
    [SerializeField] string Form_DatabaseNameVersion;
    [SerializeField] string ProgressorNameVersion;
    [SerializeField] string PhoneNumber;
    [SerializeField] bool isAPhoneNumber;
    [SerializeField] ActionGoupType Type;

    [Header("Reports")]
    [SerializeField] List<ReportType> reportTypes = new List<ReportType>();

    [Header("TV News")]
    [SerializeField] List<TVNewType> TVNewTypes = new List<TVNewType>();

    [Header("BD Data")]
    [SerializeField] DataBaseType DBTypes;
    [SerializeField] List<DataBaseType> UpdatedDataBase = new List<DataBaseType>();
    [NonSerialized] DataBaseType CurrentDB;

    [Header("Calls")]
    [SerializeField] List<CallType> CallTypes = new List<CallType>();
    
    /*[Header("Availability call Window:")]
    [SerializeField] TimeCheckConditional StartTime;
    [SerializeField] TimeCheckConditional EndTime;*/

    [Header("Inactive Conditions")]
    [SerializeField] List<ScriptableObject> InactiveConditions = new List<ScriptableObject>();

    [Header("Erase Conditions")]
    [SerializeField] List<ConditionalClass> EraseConditions = new List<ConditionalClass>();

    [SerializeField] WordData WordThatReplaces;
    [SerializeField] bool CopyHistory;
    [SerializeField] List<DeleteCrossoutWorsd> WordsThatDeletes = new List<DeleteCrossoutWorsd>();

    [NonSerialized] List<StateEnum> stateHistory = new List<StateEnum>();
    [NonSerialized] List<StateEnum> CheckedStateHistory = new List<StateEnum>();
    [NonSerialized] List<StateEnum> DBEntryStateHistory = new List<StateEnum>();
    [NonSerialized] Dictionary<StateEnum, TimeData> StateHistoryTime = new Dictionary<StateEnum, TimeData>();
    [NonSerialized] StateEnum currentState;
    [NonSerialized] private List<ActionState> ActionsStates = new List<ActionState>();
    [NonSerialized] bool isFound;
    [NonSerialized] bool isPhoneNumberFound;
    [NonSerialized] bool isInactive;
    [NonSerialized] List<StateEnum> CurrentDoingActions = new List<StateEnum>();
    [NonSerialized] bool isPlacedInBoad;
    #region GetInputLogic

    public void InitSet()
    {
        SetCallsInfo();
        CurrentDB = DBTypes;
        DataBaseEmpy = false;
        SaveFindableAs();
        isFound = false;
        isPhoneNumberFound = false;
        isInactive = false;
        isPlacedInBoad = false;
    }

    public void disableSet()
    {
        RestartFindableAs();
    }

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

    public List<ReportType> GetListOfReportFromState(StateEnum state)
    {
        List<ReportType> auxList = new List<ReportType>();

        foreach (ReportType report in reportTypes)
        {
            if (!report) continue;
            if (auxList.Contains(report)) continue;
            if (report.GetState() == state) auxList.Add(report);
        }
        return auxList;
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

    public void SetDoingAction(StateEnum action, bool isDoing)
    {
        if(isDoing)
        {
            if (CurrentDoingActions.Contains(action)) return;
            CurrentDoingActions.Add(action);
        }
        else
        {
            if (!CurrentDoingActions.Contains(action)) return;
            CurrentDoingActions.Remove(action);
        }
    }

    public bool CheckIfActionIsDoing(StateEnum action)
    {
        foreach(StateEnum currentaction in CurrentDoingActions)
        {
            if (action == currentaction) return true;
        }
        return false;
    }

    public StateEnum GetDoingAction(int i)
    {
        return CurrentDoingActions[i];
    }

    public DataBaseType GetDB()
    {
        GetDataUpdate();
        return CurrentDB;
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

    void SetCallsInfo()
    {
        foreach (CallType call in CallTypes)
        {
            if(!call)
            {
                Debug.LogWarning("The word " + GetName() + "have a empy call");
                return;
            }
            call.SetWord(this);
        }
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
        StateEnum state = report.GetState();

        if (!stateHistory.Contains(state))
        {
            stateHistory.Add(report.GetState());
            StateHistoryTime.Add(report.GetState(), TimeManager.timeManager.GetTime());
        }

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

    public bool CheckIfStateSeenWasEntryInDB(StateEnum state)
    {
        for (int i = 0; i < DBEntryStateHistory.Count; i++)
        {
            if (DBEntryStateHistory[i] == state)
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

    public void AddStateInDBEntryStateHistory(StateEnum newState)
    {
        if (DBEntryStateHistory.Contains(newState)) return;
        DBEntryStateHistory.Add(newState);
    }

    public void AddStateInHistory(StateEnum newState)
    {
        if (stateHistory.Contains(newState)) return;
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

    private void OnEnable()
    {
        ScriptableObjectResetter.instance?.RegisterScriptableObject(this);
    }

    void IReseteableScriptableObject.ResetScriptableObject()
    {
        stateHistory.Clear();
        CheckedStateHistory.Clear();
        StateHistoryTime.Clear();
        currentState = null;
        ActionsStates.Clear();
        isFound = false;
        isPhoneNumberFound = false;
        isInactive = false;
        CurrentDoingActions.Clear();
        isPlacedInBoad = false;
        
        //Debug.Log("reseted " + name);
    }
    public bool GetInactiveState() {

     return CheckInactiveConditions();
    }
    bool CheckInactiveConditions()
    {
        if (isInactive) return true;
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
    public bool GetEraseState()
    {
        // desactivado el CheckEraseConditions()
        return false;
    }
    bool CheckEraseConditions()
    {
        if (EraseConditions.Count == 0) return false;

        foreach (ConditionalClass conditional in EraseConditions)
        {
            IConditionable auxInterface = conditional.condition as IConditionable;

            bool conditionState = auxInterface.GetStateCondition(2);

            if (!conditional.ifNot)
            {
                conditionState = !conditionState;
            }

            if (conditionState)
            {
                return false;
            }
            conditional.SetDoit(true);
        }
        
        return true;
    }





    public void SetInactive()
    {
        isInactive = true;
    }

    public DeleteCrossoutWorsd CheckOtherWordDelete(WordData word)
    {
        if (WordsThatDeletes.Count == 0) return null;

        foreach(DeleteCrossoutWorsd dw in WordsThatDeletes)
        {
            dw.SetFound();
            if (dw.GetWord() == word) return dw;
        }

        return null;
    }

    #endregion

    public void GetDataUpdate()
    {
        if (UpdatedDataBase.Count == 0) return;
        foreach(DataBaseType DB in UpdatedDataBase)
        {
            if (DB == null)
            {
                Debug.Log("The word " + GetName() + " have a null update database");
                continue;
            }
            if(DB.CheckConditionals() && !DataBaseEmpy)
            {
                CurrentDB = DB;
                //DB.SetWasSetted();
            }
        }

    }
    [NonSerialized] bool DataBaseEmpy;
    public void SetDataBase(DataBaseType DB)
    {
        DataBaseEmpy = true;
        CurrentDB = DB;
    }

    public TVNewType GetVilifiedNew() {
        TVNewType VilifiedNew = null;
       // if(TVNewTypes.Count == 0 && !GetIsAPhoneNumber()) Debug.LogWarning("the word " + GetName() + " dont have a vilified new assigned");
        foreach (TVNewType _new in TVNewTypes)
        {
            if(_new.name.ToLower().Contains("vili"))
            {
                VilifiedNew = _new;
                VilifiedNew.removeCondition();
            }
            else
            {
                //Debug.LogWarning("the word " + GetName() + " dont have a vilified new assigned");
            }
        }
        return VilifiedNew; }

    public string GetName() { return wordName; }

    public string FindFindableName(string wordCompere, bool comesFromDBTitle = false){
        string aux = "";
        string normalizedWord = Regex.Replace(wordCompere.Trim(), @"[^\w]", "");

        if (comesFromDBTitle) return Regex.Replace(GetForm_DatabaseNameVersion().Trim(), @"[^\w]", "");


        foreach (string s in FindableAs)
        {
            string normalizedFindable = Regex.Replace(s.Trim(), @"[^\w]", "");
            if (normalizedWord == normalizedFindable)
            {
                aux = normalizedFindable;
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

    public List<CallType> GetListOfCalls() { return CallTypes; }

    public void SetListOfCalls(List<CallType> list) { CallTypes = list; }

    public ActionGoupType GetActionGroupType() { return Type; }

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

    public void DeleteFoundAsWord(string foundAsTxt)
    {
        string WordToRemove = "";

        foreach(string s in FindableAs)
        {
            if(foundAsTxt == s)
            {
                WordToRemove = s;
            }
        }

        FindableAs.Remove(WordToRemove);
    }

    List<string> InitFindableAs;
    void SaveFindableAs()
    {
        InitFindableAs = new List<string>(FindableAs);
        //AddFormDataBaseOnFindableAs();
    }

    void RestartFindableAs()
    {
        FindableAs = new List<string>(InitFindableAs);
    }

    public List<string> GetFindableList() { return FindableAs; }

    void AddFormDataBaseOnFindableAs()
    {
        if (Form_DatabaseNameVersion == "") return;
        FindableAs.Add(Form_DatabaseNameVersion);
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
        return new TimeData(0,0,0);
    }

    public void SetIsFound(bool x = true)
    {
        isFound = x;
    }

    public bool GetIsFound() { return isFound;}

   public void SetPlacedInBoard()
    {
        isPlacedInBoad = true;
    }

    public bool GetPlacedInBoard() { return isPlacedInBoad; }

    public List<ReportType> GetReportList() { return reportTypes; }

    public WordData MarkWordsWordThatReplaceRetroactive()
    {
        WordData currentWord = GetWordThatReplaces();
        WordData auxWord = GetWordThatReplaces();

        while (currentWord != null)
        {
            currentWord.SetIsFound();

            auxWord = currentWord;

            currentWord = currentWord.GetWordThatReplaces();

            if (currentWord == auxWord) break;
        }

        return auxWord;
    }

    public List<WordData> SearchForWordsThatReplaceRetroactive()
    {
        List<WordData> words = new List<WordData>();

        WordData currentWord = GetWordThatReplaces();
        WordData auxWord = GetWordThatReplaces();

        while (currentWord != null)
        {
            auxWord = currentWord;

            words.Add(currentWord);

            currentWord = currentWord.GetWordThatReplaces();

            if (currentWord == auxWord) break;
        }

        return words;
    }

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

[Serializable]
public class DeleteCrossoutWorsd
{
    [SerializeField] WordData word;
    [SerializeField] deleteType DeleteType;

    public enum deleteType
    {
        Erase,
        CrossOut,
    }

    public void SetFound()
    {
        word.SetIsFound();
    }

    public WordData GetWord() { return word; }
    public bool isErase()
    {
        if (DeleteType == deleteType.Erase) return true;
        else return false;
    }
}