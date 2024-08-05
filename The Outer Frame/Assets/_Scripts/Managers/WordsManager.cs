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
    [SerializeField] bool SaveProgress;
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

    public ReportType RequestLastReport(WordData _word)
    {
        ReportType report = FindWordInList(_word).GetLastReport();
        if (!report) { Debug.LogWarning(_word.GetName() + " last state report was not assigned");}
        return report;
    }

    public ReportType RequestReport(WordData _word, StateEnum state)
    {
        ReportType report = FindWordInList(_word).GetReport(state);
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


    public void RequestChangeState(WordData _word, StateEnum WordState)
    {
        FindWordInList(_word).ChangeState(WordState);
        FindWordInList(_word).SetReactionCall(WordState);
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
        return FindWordInList(_word).GetHistorySeen();
    }

    public void CleanStateOnHistory(WordData _word, StateEnum WordState)
    {
        FindWordInList(_word).CleanStateFromHistory(WordState);
    }

    public bool CheckIfStateAreAutomaticAction(WordData _word, StateEnum WordState)
    {
        return FindWordInList(_word).CheckIfStateAreAutomaticAction(WordState);
    }

    public int GetModifyActionDuration(WordData _word, StateEnum WordState)
    {
        return FindWordInList(_word).GetModifyActionDuration(WordState);
    }
    WordData FindWordInList(WordData _word)
    {
        foreach(WordData w in wordsDic)
        {
            if(w==_word)
            {
                return w;
            }
        }
        Debug.LogWarning("The word " + _word.GetName() + " is not assigned in the Word Manager");
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
            if (NormalizeWord(w.GetFindableName()).Contains(NormalizeWord(WordToCompare)))
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
}

