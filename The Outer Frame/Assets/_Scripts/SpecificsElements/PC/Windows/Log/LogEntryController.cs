using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LogEntryController : MonoBehaviour
{
    [SerializeField] TMP_Text SubjectTxt;
    [SerializeField] TMP_Text ActionTxt;
    [SerializeField] TMP_Text DateTxt;
    [SerializeField] GameObject OpenFileBtn;
    [SerializeField] GameEvent OnPressBtn;

    ReportType reportType;
    CallType callType;

    LogEntryData _data;
    public void Initialize(LogEntryData data)
    {
        reportType = data.reportType;
        callType = data.callType;

        _data = data;


        if (reportType) 
        {
            SubjectTxt.text = data.word.GetDatabaseNameVersion();
        }
        else
        {
            SubjectTxt.text = WordsManager.WM.FindWordWithPhoneNum(data.word).GetName();
            
        }
        CheckTextOverflow(SubjectTxt);
        ActionTxt.text = data.action;
        CheckTextOverflow(ActionTxt);


        TimeData actualTime = TimeManager.timeManager.GetTime();
        DateTxt.text = $"OCT 30. {actualTime.Hour:00}:{actualTime.Minute:00}"; 
    }

    public void EnableReport()
    {
        OpenFileBtn.SetActive(true);
    }

    public void ShowPanel()
    {
        OnPressBtn?.Invoke(this, _data);
    }

    public void OnUpdatePC(WordData _word)
    {
        if(reportType && checkHaveReportsToShow(_word))
        {
            OpenFileBtn.SetActive(true);
        }
        else if(callType && checkHaveTranscriptsToShow(_word))
        {
            OpenFileBtn.SetActive(true);
        }

    }

    bool checkHaveReportsToShow(WordData word)
    {
        List<StateEnum> stateHistory = WordsManager.WM.GetHistorySeen(word);

        foreach (var state in stateHistory)
        {
            List<ReportType> reports = word.GetListOfReportFromState(state);
            foreach (ReportType R in reports)
            {
                if (!R.GetwasRegisteredInDB()) continue;
                if(R == reportType) return true;
            }

        }
        return false;
    }

    bool checkHaveTranscriptsToShow(WordData word)
    {
        List<CallType> CallsHistory = WordsManager.WM.GetAllinDBCalls(word);

        foreach (var call in CallsHistory)
        {
            if(call == callType) return true;
        }
        return false;
    }

    void CheckTextOverflow(TMP_Text tmpComponent)
    {
        float containerWidth = tmpComponent.transform.parent.GetComponent<RectTransform>().rect.width;
        Vector2 preferredSize = tmpComponent.GetPreferredValues();

        if (preferredSize.x > containerWidth) TruncateTextWithEllipsis(containerWidth, tmpComponent);
        else tmpComponent.text = tmpComponent.text.Replace("...", "");
    }

    void TruncateTextWithEllipsis(float containerWidth, TMP_Text tmpComponent)
    {
        string originalText = tmpComponent.text;
        string truncatedText = originalText;

        while (tmpComponent.GetPreferredValues(truncatedText + "...").x > containerWidth && truncatedText.Length > 0)
        {
            truncatedText = truncatedText.Substring(0, truncatedText.Length - 1);
        }

        tmpComponent.text = truncatedText + "...";
    }

    public WordData GetWord()
    {
        return _data.word;
    }
    public ReportType GetReportType()
    {
        return reportType;
    }
    public CallType GetCallType()
    {
        return callType;
    }
}
