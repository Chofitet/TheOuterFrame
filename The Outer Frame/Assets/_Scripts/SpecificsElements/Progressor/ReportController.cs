using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ReportController : MonoBehaviour
{
    [SerializeField] TMP_Text reporttxt;
    [SerializeField] GameEvent OnPCReportActiualization;
    [SerializeField] float DelayToPC;
    
    public void initReport(WordData word, StateEnum state, bool isAborted, bool isAlreadyDone)
    {
        if(isAlreadyDone)
        {
            reporttxt.text = "The action \"" + state.GetActionVerb() + " " + word.GetName() + "\" has already been done";
            GetComponent<Animator>().SetTrigger("print");
            return;
        }
        if(isAborted)
        {
            reporttxt.text = "Acción " + state.GetActionVerb() + " " + word.GetName() + " Abortada con éxito";
            GetComponent<Animator>().SetTrigger("print");
            return;
        }
        reporttxt.text = WordsManager.WM.RequestReport(word,state).GetText();
        FindableWordsManager.FWM.InstanciateFindableWord(reporttxt);
        GetComponent<Animator>().SetTrigger("print");
    }

    public void OnLeveReportInPC(Component sender, object obj)
    {
        Invoke("Delay", DelayToPC);
    }

    void Delay()
    {
        OnPCReportActiualization?.Invoke(this, gameObject);
    }

    public void DeleteReport(Component sender,object obj)
    {
        if (obj == gameObject) Destroy(gameObject);
    }

}
