using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoManager : MonoBehaviour
{
    [SerializeField] bool DemoMode;
    [SerializeField] GameEvent OnGoToDemoCut;
    [SerializeField] GameEvent OnGoToDemoCutStamping;
    [SerializeField] GameEvent OnGoToDemoCutDirectly;
    [SerializeField] GameEvent OnDisableInput;

    [Header("Demo reports cut off on taken")]
    [SerializeField] List<ReportType> DemoReportsCutOffOnTaken = new List<ReportType>();
    [SerializeField] int StampsAllowAfterReportsCutOffOnTaken = 0;
    [SerializeField] float TimeToWaitBeforeReportUp = 1;
    int StampsAfterIndex = 1;

    [Header("Demo report to cut off on stamp")]
    [SerializeField] List<ReportType> DemoReportsCutOffOnStamp = new List<ReportType>();

    [Header("Demo time cut off")]
    [SerializeField] TimeCheckConditional DemoTimeCutOff;

    [Header("Demo DB search cut off")]
    [SerializeField] List<SearchedInDBConditional> SearchedInDBCutOff;


    private void OnEnable()
    {
        TimeManager.OnMinuteChange += UpdateMinuteClock;
    }
    private void OnDisable()
    {
        TimeManager.OnMinuteChange -= UpdateMinuteClock;
    }

    public void CheckFirstTakeReport(Component sender,object obj)
    {
        if (!DemoMode) return;
        
        GameObject reportSended = (GameObject)obj;

        foreach(ReportType report in DemoReportsCutOffOnTaken)
        {
            if(reportSended.GetComponent<IndividualReportController>().GetRepoertype() == report)
            {
                Invoke("SendOnGoToDemoCutDirectly", TimeToWaitBeforeReportUp);
            }
        }
    }

    void SendOnGoToDemoCutDirectly()
    {
        OnDisableInput?.Invoke(this, null);
        OnGoToDemoCut?.Invoke(this, null);
        OnGoToDemoCutDirectly?.Invoke(this, null);
    }
    void SendOnGoToDemoCutStamping()
    {
        OnDisableInput?.Invoke(this, null);
        OnGoToDemoCut?.Invoke(this, null);
        OnGoToDemoCutStamping?.Invoke(this, null);
    }

    bool onceTimeToCutDemo;
    void UpdateMinuteClock()
    {
        if (!DemoMode) return;
        if (DemoTimeCutOff == null) return;
        if (DemoTimeCutOff.GetStateCondition() && !onceTimeToCutDemo)
        {
            SendOnGoToDemoCutDirectly();
            onceTimeToCutDemo = true;
        }
    }

    public void InactiveCutDemo(Component sender, object obj)
    {
        DemoMode = false;
    }

    public void CheckOnStamp(Component sender, object obj)
    {
        DataFromActionPlan dataFromActionPlan = (DataFromActionPlan)obj;

        CheckForAStampReportToGoToDemoCutOff(DemoReportsCutOffOnStamp, dataFromActionPlan.word, dataFromActionPlan.state); // corta con un reporte específico

        if(CheckDemoReportsCutOffonTakenWasDone()) // corta con la siguiente accion desde que un reporte fue hecho
        {
            if(StampsAllowAfterReportsCutOffOnTaken == StampsAfterIndex)SendOnGoToDemoCutStamping();

            StampsAfterIndex += 1;
        }
    }

    public void CheckOnSearchInDB(Component sender, object obj)
    {
        foreach(SearchedInDBConditional conditional in SearchedInDBCutOff)
        {
            if(conditional.GetStateCondition())
            {
                SendOnGoToDemoCutDirectly();
            }
        }
    }

    void CheckForAStampReportToGoToDemoCutOff(List<ReportType> GoToCreditsReports, WordData word, StateEnum state)
    {
        foreach (ReportType data in GoToCreditsReports)
        {
            WordData _word = word;

            if (state.GetSpecialActionWord()) _word = state.GetSpecialActionWord();

            ReportType reportToShow = WordsManager.WM.RequestReport(_word, state);


            if (reportToShow == data)
            {
                SendOnGoToDemoCutStamping();
            }
        }
    }


    bool CheckDemoReportsCutOffonTakenWasDone()
    {
        foreach(ReportType report in DemoReportsCutOffOnTaken)
        {
            if(report.GetWasSet()) return true;
        }
        return false;
    }

}
