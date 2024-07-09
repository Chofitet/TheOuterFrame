using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ReportController : MonoBehaviour
{
    [SerializeField] TMP_Text reporttxt;
    
    public void initReport(WordData word, StateEnum state, bool isAborted)
    {
        if(!isAborted)
        {
            reporttxt.text = "Acción " + state.GetActionVerb() + " " + word.GetName() + " Abortada con éxito";
            GetComponent<Animator>().SetTrigger("print");
            return;
        }
        reporttxt.text = WordsManager.WM.RequestLastReport(word).GetText();
        FindableWordsManager.FWM.InstanciateFindableWord(reporttxt);
        GetComponent<Animator>().SetTrigger("print");
    }

}
