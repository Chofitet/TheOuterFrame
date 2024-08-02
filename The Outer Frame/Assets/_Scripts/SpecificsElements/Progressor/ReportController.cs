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
        if(!WordsManager.WM.RequestReport(word, state))
        {
            reporttxt.text = "The report of " + state.GetActionVerb() + " not assigned in " + word.GetName();
            GetComponent<Animator>().SetTrigger("print");
            return;
        }
        if (isAlreadyDone)
        {
            reporttxt.text = "The action \"" + state.GetActionVerb() + " " + word.GetName() + "\" has already been done";
            GetComponent<Animator>().SetTrigger("print");
            return;
        }
        if(isAborted)
        {
            reporttxt.text = "The action \"" + state.GetActionVerb() + " " + word.GetName() + "\" was aborted succesfully";
            GetComponent<Animator>().SetTrigger("print");
            return;
        }
        reporttxt.text = WordsManager.WM.RequestReport(word,state).GetText();
        FindableWordsManager.FWM.InstanciateFindableWord(reporttxt);
        GetComponent<Animator>().SetTrigger("print");
    }

    void Print()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        
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
