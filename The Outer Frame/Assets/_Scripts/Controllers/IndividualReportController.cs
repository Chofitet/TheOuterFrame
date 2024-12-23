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
    [SerializeField] GameEvent OnReactiveIdeaPosit;
    [SerializeField] GameEvent OnGrabFinalReport;
    [SerializeField] GameEvent OnTakeSecondToLastReport;
    [SerializeField] GameEvent OnActionRejected;
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
        if (report.GetIsTheLastReport()) return;
        if (report.GetDeleteDBRepoert())
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

    public ReportType GetRepoertype() { return report; }

    public void OnSendReportAutomatically(Component sender, object obj)
    {
        if (!report.GetDeleteDBRepoert() || !wasTaken) return;
        FinishReport();
    }

    bool wasTaken;
    public void OnTakeReport(Component sender, object obj)
    {
        if ((GameObject)obj != gameObject) return;

        wasTaken = true;

        if (!isComplete)
        {
            OnActionRejected?.Invoke(this, report.GetAction());
            return;
        }

        OnReactiveIdeaPosit.Invoke(this, report.GetAction());

        if (report.GetDeleteDBRepoert()) OnGrabFinalReport?.Invoke(this, null);

        if (report.GetSecondToLastReport())
        {
            OnTakeSecondToLastReport?.Invoke(this, null);
        }
    }

    

}
