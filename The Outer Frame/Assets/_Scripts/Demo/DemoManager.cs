using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoManager : MonoBehaviour
{
    [SerializeField] bool DemoMode;
    [SerializeField] GameEvent OnGoToDemoCut;
    [SerializeField] GameEvent OnDisableInput;

    [Header("Demo reports cut off")]
    [SerializeField] List<ReportType> DemoReportsCutOff = new List<ReportType>();
    [SerializeField] float TimeToWaitBeforeReportUp = 1;

    [Header("Demo time cut off")]
    [SerializeField] TimeCheckConditional DemoTimeCutOff;


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

        foreach(ReportType report in DemoReportsCutOff)
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
        if (!DemoMode) return;
        if (DemoTimeCutOff == null) return;
        if (DemoTimeCutOff.GetStateCondition() && !onceTimeToCutDemo)
        {
            OnDisableInput?.Invoke(this, null);
            OnGoToDemoCut?.Invoke(this, null);
            onceTimeToCutDemo = true;
        }
    }

    public void InactiveCutDemo(Component sender, object obj)
    {
        DemoMode = false;
    }

}
