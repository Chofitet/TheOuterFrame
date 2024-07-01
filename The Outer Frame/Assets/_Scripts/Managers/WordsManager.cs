using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordsManager : MonoBehaviour
{
    //Sistema encargado de solicitar requests a Words específicas para que devuelvan un input
    [SerializeField] List<WordData> wordsDic = new List<WordData>();
    [SerializeField] bool SaveProgress;
    public static WordsManager WM { get; private set; }
    public event Action<WordData> OnChangeStateOfWord;
    public event Action<WordData> OnChangeStateSeenOfWord;


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
        foreach(WordData word in wordsDic)
        {
            word.CleanHistory();
        }
    }

    public ReportType RequestLastReport(WordData _word)
    {
        return FindWordInList(_word).GetLastReport();
    }

    public ReportType RequestReport(WordData _word, StateEnum state)
    {
        return FindWordInList(_word).GetReport(state);
    }

    public TVNewType RequestNew(WordData _word, StateEnum state)
    {
        return FindWordInList(_word).GetTVnew(state);
    }

    public DataBaseType RequestBDWikiData(WordData _word)
    {
        return FindWordInList(_word).GetDB();
    }

    public void RequestChangeState(WordData _word, StateEnum WordState)
    {
        FindWordInList(_word).ChangeState(WordState);
        OnChangeStateOfWord?.Invoke(_word);
    }

    public void RequestChangeStateSeen(WordData _word, StateEnum WordState)
    {
        FindWordInList(_word).CheckStateSeen(WordState);
        OnChangeStateSeenOfWord?.Invoke(_word);
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

    WordData FindWordInList(WordData _word)
    {
        foreach(WordData w in wordsDic)
        {
            if(w==_word)
            {
                return w;
            }
        }

        return wordsDic[0];
    }

    public bool GetInactiveState(WordData _word)
    {
        return FindWordInList(_word).GetInactiveState();
    }

}
