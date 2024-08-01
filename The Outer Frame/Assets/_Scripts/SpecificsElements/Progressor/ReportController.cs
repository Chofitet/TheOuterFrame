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
        transform.GetChild(0).gameObject.SetActive(false);
        if (isAlreadyDone)
        {
            reporttxt.text = "The action \"" + state.GetActionVerb() + " " + word.GetName() + "\" has already been done";
            Invoke("Print", 0.5f);
            return;
        }
        if(isAborted)
        {
            reporttxt.text = "Acción " + state.GetActionVerb() + " " + word.GetName() + " Abortada con éxito";
            Invoke("Print", 0.5f);
            return;
        }
        reporttxt.text = WordsManager.WM.RequestReport(word,state).GetText();
        FindableWordsManager.FWM.InstanciateFindableWord(reporttxt);
        Invoke("Print", 0.5f);
    }

    void Print()
    {
        transform.GetChild(0).gameObject.SetActive(true);
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
    
    public void OnTakeReport(Component sender, object obj)
    {
        GetComponent<BoxCollider>().enabled = true;
    }

}
