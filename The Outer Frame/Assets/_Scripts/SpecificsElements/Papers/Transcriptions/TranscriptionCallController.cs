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
    [SerializeField] GameObject InterseptedInfoPanel;
    [SerializeField] TMP_Text txtFromNoIntercepted;
    [SerializeField] GameObject UploadBTN;
    [SerializeField] GameObject DisposeBTN;

    CallType call;

    public void Inicialization(CallType _call, WordData word)
    {
        if(!_call)
        {
            txtCall.text = "#[NO LINE ACTIVITY DETECTED]";
            DisposeBTN.SetActive(true);
            UploadBTN.SetActive(false);
            InterseptedInfoPanel.SetActive(true);
            InfoPanel.SetActive(false);
            txtFromNoIntercepted.text = word.GetPhoneNumber();
            return;
        }

        call = _call;

        txtCall.text = call.GetDialogue();
        txtFrom.text = word.GetPhoneNumber();
        FindableWordsManager.FWM.InstanciateFindableWord(txtFrom,FindableBtnType.FindableBTN);
        //txtTo.text = call.GetTo();
        //FindableWordsManager.FWM.InstanciateFindableWord(txtTo,FindableBtnType.FindableBTN);

        txtAt.text = $"{_call.GetCachedStartTime().ToString()} to {_call.GetCachedFinishTime().ToString()}";
        GetComponent<IndividualCallController>().SetType(true, call);
        FindableWordsManager.FWM.InstanciateFindableWord(txtCall,FindableBtnType.FindableBTN);
        DisposeBTN.SetActive(false);
        UploadBTN.SetActive(true);

    }

    public void EnterDataBase() => call?.SetWasEnterToDataBase(true); 

    public void DestroyTranscription(Component sender, object obj)
    {
        call?.SetWasEnterToDataBase(true);
        Invoke("delay", 0.1f);
    }

    void delay()
    {
        Destroy(gameObject);
    }

     
}
