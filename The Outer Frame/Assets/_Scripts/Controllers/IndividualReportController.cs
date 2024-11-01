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
    [SerializeField] GameObject Photo1;
    [SerializeField] GameEvent OnTakePhotoReport;
    [SerializeField] GameEvent OnTakeFinalReport;
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

        if (report.GetFinalReport())
        {
            OnTakeFinalReport?.Invoke(this, null);
        }

        if(report.GetTriggerDrawerAnim())
        {
            OnTakePhotoReport?.Invoke(this, Photo1);
        }

        if (isComplete && !report.GetTriggerDrawerAnim())
        {
            UpdatePCDatabase?.Invoke(this, word);
            word.AddStateInDBEntryStateHistory(report.GetState());
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
