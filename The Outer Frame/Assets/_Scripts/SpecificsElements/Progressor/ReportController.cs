using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;

public class ReportController : MonoBehaviour
{
    [SerializeField] TMP_Text Resulttxt;
    [SerializeField] TMP_Text ActionCalltxt;
    [SerializeField] TMP_Text Statustxt;
    [SerializeField] GameEvent OnMovePaperToTakenPos;
    [SerializeField] float DelayToPC;
    
    
    public void initReport(WordData word, ReportType report, bool isAborted, bool isAlreadyDone, TimeData timeComplete)
    {

        bool isNotCompleted = false;
        string status = "Completed";
        StateEnum state = report.GetAction();
        string Name = word.GetForm_DatabaseNameVersion();
        string actionVerb = state.GetInfinitiveVerb();

        if (!report)
        {
            Resulttxt.text = "No report not assigned in " + Name;
            status = "a";
            isNotCompleted = true;
        }
        if (isAlreadyDone)
        {
            Resulttxt.text = report.GetTextForRepetition();
            if (report.GetTextForRepetition() == "") Debug.LogWarning("No text for repetition in report: " + report.name);
            status = "Rejected";
            isNotCompleted = true;
        }
        if(isAborted)
        {
            Resulttxt.text = "The action \"" + actionVerb + " " + Name + "\" was aborted succesfully";
            status = "Aborted";
            isNotCompleted = true;
        }

        if(report.GetIsAutomatic()) status = "Rejected";

        ActionCalltxt.text = actionVerb + " " + DeleteSpetialCharacter(Name);
        Statustxt.text = status + " at OCT 30th " + $"{timeComplete.Hour:00}:{timeComplete.Minute:00}";

        GetComponent<IndividualReportController>().SetType(false, word, report);

        if (isNotCompleted) return;
        Resulttxt.text = report.GetText();
        FindableWordsManager.FWM.InstanciateFindableWord(Resulttxt);
        GetComponent<IndividualReportController>().SetType(true, word, report);

    }

    string DeleteSpetialCharacter(string txt)
    {
        return Regex.Replace(txt, @"[\?\.,\n\r]", "");
    }

    public void OnTakeReport(Component sender, object obj)
    {
        GetComponent<BoxCollider>().enabled = true;
        OnMovePaperToTakenPos?.Invoke(this, gameObject);
        Destroy(this);
    }

}
