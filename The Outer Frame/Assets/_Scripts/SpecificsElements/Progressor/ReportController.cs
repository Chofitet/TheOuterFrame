using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ReportController : MonoBehaviour
{
    [SerializeField] TMP_Text reporttxt;
    [SerializeField] GameEvent OnPCReportActiualization;
    
    public void initReport(WordData word, StateEnum state, bool isAborted, bool isNotPossible)
    {
        if(isNotPossible)
        {
            reporttxt.text = "Acción " + state.GetActionVerb() + " " + word.GetName() + " no es posible de realizar";
            GetComponent<Animator>().SetTrigger("print");
            return;
        }
        if(isAborted)
        {
            reporttxt.text = "Acción " + state.GetActionVerb() + " " + word.GetName() + " Abortada con éxito";
            GetComponent<Animator>().SetTrigger("print");
            return;
        }
        reporttxt.text = WordsManager.WM.RequestLastReport(word).GetText();
        FindableWordsManager.FWM.InstanciateFindableWord(reporttxt);
        GetComponent<Animator>().SetTrigger("print");
    }

    public void OnLeveReportInPC(Component sender, object obj)
    {
        OnPCReportActiualization?.Invoke(this, gameObject);
    }

    public void DeleteReport(Component sender,object obj)
    {
        if (obj == gameObject) Destroy(gameObject);
    }

}
