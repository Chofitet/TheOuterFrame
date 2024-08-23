using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndividualReportController : MonoBehaviour
{
    bool isComplete;
    WordData word;
    ReportType report;
    [SerializeField] GameEvent UpdatePCDatabase;
    public void SetType(bool x, WordData _word ,ReportType _report)
    {
        report = _report;
        word = _word;
        
        if (x && !report.GetIsAutomatic())
        {
            isComplete = x;
        }
    }

    public void FinishReport()
    {
        if (isComplete)
        {
            UpdatePCDatabase?.Invoke(this, gameObject);
            WordsManager.WM.RequestChangeStateSeen(word, report.GetState());
        }
        Destroy(gameObject);
    }
}
