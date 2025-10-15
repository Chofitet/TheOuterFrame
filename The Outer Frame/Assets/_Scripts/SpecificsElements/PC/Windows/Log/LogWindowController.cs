using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogWindowController : MonoBehaviour
{
    [SerializeField] GameObject PrefabLogReport;
    [SerializeField] GameObject PrefabLogTranscript;
    [SerializeField] GameObject Grid;
    [SerializeField] GameObject TranscriptBTN;
    [SerializeField] GameObject ReportBTN;
    [SerializeField] GameObject LogBTN;
    List<LogEntryController> LogEntries = new List<LogEntryController>();
    

    public void AddEntry(Component sender, object obj)
    {
        LogEntryData data = (LogEntryData)obj;

        if(data.reportType)
        {
            AddReportLog(data);
        }
        else if(data.callType)
        {
            AddTranscriptionLog(data);
        }
    }

    void AddReportLog(LogEntryData data)
    {
         instanciateEntry(PrefabLogReport,data);
    }

    void AddTranscriptionLog(LogEntryData data)
    {
        instanciateEntry(PrefabLogTranscript,data);
    }

    void instanciateEntry(GameObject prefab, LogEntryData data)
    {
        GameObject log = Instantiate(prefab);
        log.GetComponent<LogEntryController>().Initialize(data);
        LogEntries.Add(log.GetComponent<LogEntryController>());
        log.transform.SetParent(Grid.transform, false);
        log.transform.SetSiblingIndex(0);
    }

    public void OnPcUpdate(Component sender, object obj)
    {
        WordData word = (WordData)obj;
        foreach(LogEntryController entry in LogEntries)
        {
            entry.OnUpdatePC(word);
        }
    }

    // Llamado desde los botones de la palabra
    public void AddFilters(Component sender, object obj)
    {
        SearchLogData data = (SearchLogData)obj;

        WordData word = data.word;
        LogFilterType actualFilter = data.filterType;
        ActiveAllEntries();

        if (actualFilter == LogFilterType.log) return;

        ApplyWordFilter(word);
        ApplyTypeFilter(actualFilter);
    }

    void ApplyWordFilter(WordData word)
    {
        foreach (LogEntryController entry in LogEntries)
        {
            if (entry.GetWord() != WordsManager.WM.FindWordWithPhoneNum(word)) entry.gameObject.SetActive(false);
        }
    }

    void ApplyTypeFilter(LogFilterType filterType)
    {
        foreach (LogEntryController entry in LogEntries)
        {
            if (filterType == LogFilterType.report && !entry.GetReportType()) entry.gameObject.SetActive(false);
            if (filterType == LogFilterType.transcript && !entry.GetCallType()) entry.gameObject.SetActive(false);
        }
    }

    public void ActiveAllEntries()
    {
        foreach (LogEntryController entry in LogEntries)
        {
            entry.gameObject.SetActive(true);
        }
    }
}


public class LogEntryData
{
    public WordData word;
    public string action;
    public ReportType reportType;
    public CallType callType;

    public LogEntryData(WordData _word, string _action, ReportType _reportType = null, CallType _callType = null)
    {
        word = _word;
        action = _action;
        reportType = _reportType;
        callType = _callType;
    }
}

public class SearchLogData
{
    public WordData word;
    public LogFilterType filterType;

    public SearchLogData(WordData _word, LogFilterType _filter)
    {
        word = _word;
        filterType = _filter;
    }
}
[Serializable]
public enum LogFilterType
{
    log,
    report,
    transcript
}