using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LogWindowController : MonoBehaviour
{
    [SerializeField] GameObject PrefabLogReport;
    [SerializeField] GameObject PrefabLogTranscript;
    [SerializeField] GameObject Grid;
    [SerializeField] GameObject SubjectFilesBTN;
    List<LogEntryController> LogEntries = new List<LogEntryController>();
    [SerializeField] GameEvent OnActiveSubjectFilesBTN;

    [SerializeField] TMP_Text filterTag;

    [SerializeField] GameObject LogContent;
    [SerializeField] RectTransform panel;

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
        filterTag.text = word.GetForm_DatabaseNameVersion();
        StartCoroutine(RefreshUILog(true));
        //ApplyTypeFilter(actualFilter);
    }

    void ApplyWordFilter(WordData word)
    {
        foreach (LogEntryController entry in LogEntries)
        {
            WordData EntryWord = entry.GetWord();
            WordData SearchedWord = word;
            if (!SearchedWord.GetIsAPhoneNumber()) EntryWord = WordsManager.WM.FindWordWithPhoneNum(EntryWord);

            if (EntryWord != SearchedWord) entry.gameObject.SetActive(false);
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

    public void OnActiveAllEntries(Component sender, object obj)
    {
        ActiveAllEntries();
    }

    public void ActiveAllEntries()
    {
        foreach (LogEntryController entry in LogEntries)
        {
            entry.gameObject.SetActive(true);
        }
        StartCoroutine(RefreshUILog(false));
    }

    IEnumerator RefreshUILog(bool isFilter)
    { 
        LogContent.SetActive(false);
        yield return new WaitForSeconds(0.2f);
        LogContent.SetActive(true);
        if (!isFilter)
        {
            Vector2 offsetMax = panel.offsetMax;
            offsetMax.y = 6.12f; 
            panel.offsetMax = offsetMax;
        }
        else
        {
            Vector2 offsetMax = panel.offsetMax;
            offsetMax.y = 0;
            panel.offsetMax = offsetMax;
        }
    }

    public void ThereAreEntriesForWord(Component sender,object obj)
    {
        WordData wordData = (WordData)obj;

        foreach(LogEntryController entry in LogEntries)
        {
            if (entry.GetWord() == wordData) OnActiveSubjectFilesBTN?.Invoke(this, null);
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