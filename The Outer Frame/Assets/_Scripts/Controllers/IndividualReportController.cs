using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndividualReportController : MonoBehaviour
{
    bool isComplete;
    WordData word;
    ReportType report;
    [SerializeField] GameEvent UpdatePCDatabase;
    [SerializeField] GameEvent OnDescartReport;
    [SerializeField] GameEvent OnBackToGeneralView;
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
            UpdatePCDatabase?.Invoke(this, word);
            report.setwasRegisteredInDB();
        }
        else
        {
            OnDescartReport?.Invoke(this, null);
        }
        StartCoroutine(delay());
        OnBackToGeneralView?.Invoke(this, null);
    }

    IEnumerator delay()
    {
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }

}
