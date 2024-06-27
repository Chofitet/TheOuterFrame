using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Word", menuName ="Word")]
public class WordData : ScriptableObject
{
    [Header("Word Name")]
    [SerializeField] string wordName;

    [Header("Action Plan Results")]
    [SerializeField] List<ReportType> reportTypes = new List<ReportType>();

    [Header("TV News")]
    [SerializeField] List<TVNewType> TVNewTypes = new List<TVNewType>();

    [Header("BD Data")]
    [SerializeField] DataBaseType DBTypes;

    [Header("Exceptions")]
    [SerializeField] List<Exceptions> exceptions = new List<Exceptions>();

    private List<StateEnum> stateHistory = new List<StateEnum>();
    private List<StateEnum> CheckedStateHistory = new List<StateEnum>();
    private Dictionary<StateEnum, TimeData> StateHistoryTime = new Dictionary<StateEnum, TimeData>();
    StateEnum currentState;

    public void ChangeState(StateEnum newState)
    {

        if (CheckIfStateWasDone(newState))
        {
            Debug.LogWarning("The state " + newState.name + " was done");
            return;
        }

        Exceptions exception = GetExceptions(newState);

        if (exception.GetState().name != "none")
        {
            currentState = exception.GetState();
            
            if(currentState != newState)
            {
                if(exception.GetAlsoSetDefaultState())
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

    public void CheckStateSeen(StateEnum newState)
    {
        if (CheckIfStateSeenWasDone(newState))
        {
            Debug.LogWarning("The state " + newState.name + " was seen");
            return;
        }
        CheckedStateHistory.Add(newState);
    }

    public string GetName() { return wordName; }

    public Exceptions GetExceptions(StateEnum state)
    {
        return FindException(state);
    }
    
    public TVNewType GetTVnew(StateEnum state)
    {
        return FindInputInList(TVNewTypes, state);
    }

    public ReportType GetReport(StateEnum state, bool isSetTime = false)
    {
        ReportType input = FindInputInList(reportTypes, state);
        if(isSetTime) input.GetTimeWhenWasDone();
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

    T  FindInputInList<T>(List<T> list, StateEnum state) where T : IStateComparable
    {
        T aux = list[0];

        foreach(T input in list)
        {
           if (state == input.GetState())
           {
                aux = input;
           }
        }
        return aux;
    }

    public List<StateEnum> GetHistory()
    {
        return CheckedStateHistory;
    }

    Exceptions FindException ( StateEnum state)
    {
        Exceptions aux = exceptions[0];

        foreach(Exceptions ex in exceptions)
        {
            if (state == ex.GetStateDefault())
            {
                if(state != ex.GetState())
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

    public TimeData GetTimeOfState(StateEnum state)
    {
        foreach(StateEnum s in StateHistoryTime.Keys)
        {
            if (s == state)
            {
                return StateHistoryTime[s];
            }
        }
        return TimeManager.timeManager.GetTime();
    }

    public void CleanHistory()
    {
        stateHistory.Clear();
        CheckedStateHistory.Clear();
    }


}
