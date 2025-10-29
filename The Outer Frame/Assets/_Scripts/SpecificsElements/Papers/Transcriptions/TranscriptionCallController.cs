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

    CallType call;

    public void Inicialization(CallType _call, WordData word)
    {
        if(!_call)
        {
            txtCall.text = "#[NO LINE ACTIVITY DETECTED]";
            txtBTN.text = "DISPOSE";
            InterseptedInfoPanel.SetActive(true);
            InfoPanel.SetActive(false);
            txtFromNoIntercepted.text = word.GetPhoneNumber();
            return;
        }

        call = _call;

        txtCall.text = call.GetDialogue();
        txtFrom.text = call.GetFrom();
        FindableWordsManager.FWM.InstanciateFindableWord(txtFrom,FindableBtnType.FindableBTN);
        //txtTo.text = call.GetTo();
        //FindableWordsManager.FWM.InstanciateFindableWord(txtTo,FindableBtnType.FindableBTN);

        txtAt.text = $"{_call.GetCachedStartTime().ToString()} to {_call.GetCachedFinishTime().ToString()}";
        GetComponent<IndividualCallController>().SetType(true, call);
        FindableWordsManager.FWM.InstanciateFindableWord(txtCall,FindableBtnType.FindableBTN);
        txtBTN.text = "UPDATE DB";

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

     TimeData SubtractMinutesFromTime(TimeData time, int minutesToSubtract)
    {
        int totalMinutes = time.Minute - minutesToSubtract;

        // Si los minutos son negativos, calculamos las horas que hay que restar
        int extraHours = 0;
        while (totalMinutes < 0)
        {
            totalMinutes += 60;
            extraHours++;
        }

        int finalMinutes = totalMinutes;

        int totalHours = time.Hour - extraHours;
        int extraDays = 0;
        while (totalHours < 0)
        {
            totalHours += 24;
            extraDays++;
        }

        int finalHours = totalHours;
        int finalDays = time.Day - extraDays;

        // No hacemos wrap-around de días negativos (depende de tu lógica de tiempo)
        return new TimeData
        {
            Day = finalDays,
            Hour = finalHours,
            Minute = finalMinutes
        };
    }
}
