using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PCReportController : MonoBehaviour
{
    [SerializeField] TMP_Text Actiontxt;
    [SerializeField] TMP_Text Timetxt;
    [SerializeField] GameEvent OnPressPCReportBTN;

    ReportType _input;

    public void Inicialization(WordData _word, ReportType report)
    {
        if (!_word) return;
        Actiontxt.text = report.GetAction().GetActionedVerb();
        Timetxt.text = report.GetTimeWhenWasDone().ToString();
        _input = report;
    }

    public void ShowPanelWithInput()
    {
        OnPressPCReportBTN?.Invoke(this, _input);
    }

}
