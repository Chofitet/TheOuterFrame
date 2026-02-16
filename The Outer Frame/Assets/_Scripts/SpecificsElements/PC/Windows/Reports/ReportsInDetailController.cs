using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReportsInDetailController : MonoBehaviour
{

    [SerializeField] GameObject panelReporte;
    [SerializeField] ReportController ReportToFill;
    [SerializeField] GameEvent OnSearchWordInWiki;
    [SerializeField] GameEvent OnWikiWindow;
    [SerializeField] Scrollbar scrollbar;
    LogEntryData data;
    //OnPressReportBTN
    public void SetPanelText(Component sender, object obj)
    {
        panelReporte.SetActive(true);

        data = (LogEntryData)obj;

        ReportType report = data.reportType;
        ReportToFill._Reset();
        ReportToFill.initReport(data.word, report, false, false, false, null,false, report.GetTimeWhenWasDone(), new TimeData(0, 0, 0),-1);
        scrollbar.value = 1;
    }

    public void QuitPanelReport()
    {
        panelReporte.SetActive(false);
    }

    public void OnQuitPanelReport(Component sender, object obj)
    {
        panelReporte.SetActive(false);
    }

    public void GoToSubject()
    {
        OnSearchWordInWiki?.Invoke(this, data.word);
        OnWikiWindow?.Invoke(this, null);
        QuitPanelReport();
    }
}

