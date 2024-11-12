using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TranscriptionCallController : MonoBehaviour
{
    [SerializeField] TMP_Text txtCall;
    [SerializeField] GameObject InfoPanel;
    [SerializeField] TMP_Text txtFrom;
    [SerializeField] TMP_Text txtTo;
    [SerializeField] TMP_Text txtAt;
    [SerializeField] TMP_Text txtBTN;
    [SerializeField] GameObject InterseptedInfoPanel;
    [SerializeField] TMP_Text txtFromNoIntercepted;

    public void Inicialization(CallType call, WordData word)
    {
        if(!call)
        {
            txtCall.text = "- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - ";
            txtBTN.text = "DISPOSE";
            InterseptedInfoPanel.SetActive(true);
            InfoPanel.SetActive(false);
            txtFromNoIntercepted.text = word.GetPhoneNumber();
            return;
        }

        txtCall.text = call.GetDialogue();
        txtFrom.text = call.GetFrom();
        FindableWordsManager.FWM.InstanciateFindableWord(txtFrom,FindableBtnType.FindableBTN);
        //txtTo.text = call.GetTo();
        //FindableWordsManager.FWM.InstanciateFindableWord(txtTo,FindableBtnType.FindableBTN);
        txtAt.text = TimeManager.timeManager.GetTime().ToString() + " 30TH OCT";
        GetComponent<IndividualCallController>().SetType(true, call);
        FindableWordsManager.FWM.InstanciateFindableWord(txtCall,FindableBtnType.FindableBTN);
        txtBTN.text = "UPDATE DB";

    }

    public void DestroyTranscription(Component sender, object obj)
    {
        Invoke("delay", 0.1f);
    }

    void delay()
    {
        Destroy(gameObject);
    }

}
