using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ReportController : MonoBehaviour
{
    [SerializeField] TMP_Text Resulttxt;
    [SerializeField] TMP_Text ActionCalltxt;
    [SerializeField] TMP_Text Statustxt;
    [SerializeField] GameEvent OnPCReportActiualization;
    [SerializeField] float DelayToPC;
    
    
    public void initReport(WordData word, StateEnum state, bool isAborted, bool isAlreadyDone, TimeData time)
    {
        bool isNotCompleted = false;
        string status = "Completed";
        
        if (!WordsManager.WM.RequestReport(word, state))
        {
            Resulttxt.text = "The report of " + state.GetActionVerb() + " not assigned in " + word.GetName();
            status = "a";
            GetComponent<Animator>().SetTrigger("print");
            isNotCompleted = true;
        }
        if (isAlreadyDone)
        {
            Resulttxt.text = "The action \"" + state.GetActionVerb() + " " + word.GetName() + "\" has already been done";
            status = "Rejected";
            GetComponent<Animator>().SetTrigger("print");
            isNotCompleted = true;
        }
        if(isAborted)
        {
            Resulttxt.text = "The action \"" + state.GetActionVerb() + " " + word.GetName() + "\" was aborted succesfully";
            status = "Aborted";
            GetComponent<Animator>().SetTrigger("print");
            isNotCompleted = true;
        }

        if(WordsManager.WM.CheckIfStateAreAutomaticAction(word,state)) status = "Rejected";

        ActionCalltxt.text = state.GetActionVerb() + " " + word.GetName();
        Statustxt.text = status + " at OCT 30th " + $"{time.Hour:00}:{time.Minute:00}"; 

        if (isNotCompleted) return;
        Resulttxt.text = WordsManager.WM.RequestReport(word,state).GetText();
        FindableWordsManager.FWM.InstanciateFindableWord(Resulttxt);


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
