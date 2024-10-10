using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class WordsManager : MonoBehaviour
{
    //Sistema encargado de solicitar requests a Words específicas para que devuelvan un input
    [SerializeField] List<WordData> wordsDic = new List<WordData>();
    [SerializeField] List<StateEnum> Actions = new List<StateEnum>();
    [SerializeField] bool SaveProgress;
    [SerializeField] GameEvent OnRemoveEraceInstance;
    [SerializeField] GameEvent OnChangeStateOfWord;
    [SerializeField] GameEvent OnChangeStateSeenOfWord;
    public static WordsManager WM { get; private set; }


    public List<WordData> GetWordsDic() { return wordsDic; }
    private void Awake()
    {
        if (WM != null && WM != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            WM = this;
        }

        if (SaveProgress) return;
        if(wordsDic.Count == 0)
        {
            Debug.LogWarning("No word assigned to " + this.name);
            return;
        }
        foreach(WordData word in wordsDic)
        {
            word.CleanHistory();
            word.SetIsFound(false);
            
        }
    }

    private void Start()
    {
        foreach (WordData word in wordsDic)
        {
            word.InitSet();
            word.SetListOfActions(Actions);
        }
    }
    public ReportType RequestReport(WordData _word, StateEnum state)
    {
        ReportType report = FindWordInList(_word).GetReport(state);
        if (!report) Debug.LogWarning(_word.GetName() + " " + state.name + " report was not assigned");
        return report;
    }

    public ReportType RequestSpecificReport(WordData _word, StateEnum state)
    {
        ReportType report = FindWordInList(_word).GetReportFromState(state);
        if (!report) Debug.LogWarning(_word.GetName() + " " + state.name + " report was not assigned");
        return report;
    }

    public TVNewType RequestNew(WordData _word, StateEnum state)
    {
        TVNewType _new = FindWordInList(_word).GetTVnew(state);
        if (!_new) Debug.LogWarning(_word.GetName() + " " + state.name + " new was not assigned");
        return _new;
    }

    public DataBaseType RequestBDWikiData(WordData _word)
    {
        DataBaseType DB = FindWordInList(_word).GetDB();
        if (!DB) Debug.LogWarning(_word.GetName() + " Data Base was not assigned");
        return DB;
    }

    public List<CallType> RequestCall(WordData _word)
    {
        List < CallType > calls = FindWordInList(_word).GetCall();
        if (calls.Count == 0) Debug.LogWarning(_word.GetName() + " has no more calls");
        return calls;
    }
    public List<CallType> GetAllCathedCalls(WordData word)
    {
        return FindWordInList(word).GetAllCathedCalls();
    }


    public void RequestChangeState(WordData _word, ReportType report)
    {
        FindWordInList(_word).ChangeState(report);
        FindWordInList(_word).SetReactionCall(report.GetState());
        //OnChangeStateOfWord?.Invoke(this,_word);
    }

    public void RequestChangeStateSeen(WordData _word, StateEnum WordState)
    {
        FindWordInList(_word).CheckStateSeen(WordState);
       // OnChangeStateSeenOfWord?.Invoke(this,_word);
    }

    public bool CheckIfStateWasDone(WordData _word, StateEnum WordState)
    {
        return FindWordInList(_word).CheckIfStateWasDone(WordState);
    }

    public bool CheckIfStateSeenWasDone(WordData _word, StateEnum WordState)
    {
        return FindWordInList(_word).CheckIfStateSeenWasDone(WordState);
    }

    public TimeData RequestTimeDataOfState(WordData _word, StateEnum WordState)
    {
        return FindWordInList(_word).GetTimeOfState(WordState);
    }
    public List<StateEnum> GetHistory(WordData _word)
    {
        return FindWordInList(_word).GetHistory();
    }
    public List<StateEnum> GetHistorySeen(WordData _word)
    {
        List<StateEnum> aux = FindWordInList(_word).GetHistorySeen();
        return FindWordInList(_word).GetHistorySeen();
    }

    public void CleanStateOnHistory(WordData _word, StateEnum WordState)
    {
        FindWordInList(_word).CleanStateFromHistory(WordState);
    }

    public void AddStateOnHistory(WordData _word, StateEnum WordState)
    {
        FindWordInList(_word).AddStateInHistory(WordState);
    }

    public void AddStateOnSeenHistory(WordData _word, StateEnum WordState)
    {
        FindWordInList(_word).CheckStateSeen(WordState);

    }

    WordData FindWordInList(WordData _word)
    {
        if(!_word)
        {
            Debug.Log("The entered word are null");
            return null;
        }
        foreach(WordData w in wordsDic)
        {
            if(w==_word)
            {
                return w;
            }
        }
        if(_word != null)Debug.LogWarning("The word " + _word.GetName() + " is not assigned in the Word Manager");
        return null;
    }


    public bool GetInactiveState(WordData _word)
    {
        return FindWordInList(_word).GetInactiveState();
    }
    public WordData FindWordDataWithString(string WordToCompare)
    {
        WordData WD = null;
        foreach (WordData w in wordsDic)
        {
            if (NormalizeWord(w.FindFindableName(WordToCompare)) == (NormalizeWord(WordToCompare)))
            {
                WD = w;
            }
        }
        if(!WD) Debug.LogWarning("The word " + WordToCompare + " is not assigned in the Word Manager");
        return WD;
    }

    string NormalizeWord(string word)
    {
        return Regex.Replace(word.ToLower(), @"<\/?link>|[\?\.,\n\r\(\)\s]", "");
    }

    public void CheckEraseWordContitions(Component sender, object obj)
    {
        List<WordData> deletedWords = new List<WordData>();
        foreach (WordData _word in wordsDic)
        {

            if (deletedWords.Contains(_word)) continue;

            if (_word.GetEraseState())
            {
                deletedWords.Add(_word);
                OnRemoveEraceInstance?.Invoke(this, _word);
                continue;
            }
        }
    }
}

