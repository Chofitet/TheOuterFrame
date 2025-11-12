using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TranscriptionCallController : MonoBehaviour
{
    [SerializeField] TMP_Text txtCall;
    [SerializeField] TMP_Text txtFrom;
    [SerializeField] TMP_Text txtAt;
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
            txtAt.text = $"{TimeManager.timeManager.SubtractMinutesFromTime(TimeManager.timeManager.GetTime(),8).ToString()} to {TimeManager.timeManager.GetTime().ToString()}"; // por el momento, la ventana se crea al momento de imprimirla

            if (word) txtFrom.text = word.GetPhoneNumber();
            else txtFrom.text = "(864) 955-2236";
            return;
        }

        call = _call;

        txtCall.text = call.GetDialogue();
        txtFrom.text = word.GetPhoneNumber();
        FindableWordsManager.FWM.InstanciateFindableWord(txtFrom,FindableBtnType.FindableBTN);

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
