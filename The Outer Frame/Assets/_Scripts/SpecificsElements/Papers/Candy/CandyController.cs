using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandyController : MonoBehaviour
{
    bool isNotCompleted;
    WordData word;
    ReportType report;
    [SerializeField] GameEvent OnReactiveIdeaPosit;
    [SerializeField] GameEvent OnActionRejected;
    public void initCandy(WordData _word, ReportType _report, bool isAborted, bool isAlreadyDone, bool isTheSameAction)
    {
        if(isAlreadyDone || isAborted || isAlreadyDone || isTheSameAction )
        {
            isNotCompleted = true;
        }
        word = _word;
        report = _report;
    }

    public void OnTakeReport(Component sender, object obj)
    {
        if (!isNotCompleted)
        {
            WordsManager.WM.RequestChangeStateSeen(word, report.GetState());
        }
        else
        {
            OnActionRejected?.Invoke(this, report.GetAction());
        }
        OnReactiveIdeaPosit.Invoke(this, report.GetAction());

        Destroy(this);
    }
}
