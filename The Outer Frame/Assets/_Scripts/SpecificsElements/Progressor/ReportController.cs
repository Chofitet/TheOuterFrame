using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ReportController : MonoBehaviour
{
    [SerializeField] TMP_Text reporttxt;
    [SerializeField] GameEventListener LeaveReportListener;
    
    public void initReport(WordData word)
    {
        reporttxt.text = WordsManager.WM.RequestLastReport(word).GetText();
        FindableWordsManager.FWM.InstanciateFindableWord(reporttxt);
    }

}
