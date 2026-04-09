using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoManager : MonoBehaviour
{
    [SerializeField] bool isOnDemo;
    [SerializeField] GameEvent OnGoToDemoCut;
    [SerializeField] GameEvent OnDisableInput;
    [SerializeField] float TimeToWaitBeforeReportUp = 1;

    [Header("Reports to Cut Demo")]
    [SerializeField] List<ReportType> ReportsToTriggerDemoCut = new List<ReportType>();

    [Header("Time To Cut Demo")]
    [SerializeField] TimeCheckConditional TimeToCutDemo;


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
        if (!isOnDemo) return;
        
        GameObject reportSended = (GameObject)obj;

        foreach(ReportType report in ReportsToTriggerDemoCut)
        {
            if(reportSended.GetComponent<IndividualReportController>().GetRepoertype() == report)
            {
                Invoke("SendOnGoToDemoCut", TimeToWaitBeforeReportUp);
            }
        }
    }

    void SendOnGoToDemoCut()
    {
        OnGoToDemoCut.Invoke(this, null);
    }

    bool onceTimeToCutDemo;
    void UpdateMinuteClock()
    {
        if (!isOnDemo) return;
        if (TimeToCutDemo == null) return;
        if (TimeToCutDemo.GetStateCondition() && !onceTimeToCutDemo)
        {
            OnDisableInput?.Invoke(this, null);
            OnGoToDemoCut?.Invoke(this, null);
            onceTimeToCutDemo = true;
        }
    }

    public void InactiveCutDemo(Component sender, object obj)
    {
        isOnDemo = false;
    }

}
