using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PinchofonoManager : MonoBehaviour
{
    [SerializeField] int minutesToRecording;
    [SerializeField] TMP_Text CountDown;
    int minutePassCounter;
    WordData word;
    CallType CallToShow;
    bool isInterrupted;
    [SerializeField] GameEvent OnCallEndRecording;
    [SerializeField] GameEvent OnCallCatch;

    public void SetRecording(Component sender, object obj)
    {
        if (!WordSelectedInNotebook.Notebook.GetSelectedWord()) return;
        CallToShow = null;
        TimeManager.OnMinuteChange += CounterPassTime;
        CountDown.text = "00:" + minutesToRecording;
        word = WordSelectedInNotebook.Notebook.GetSelectedWord();
    }

    void CounterPassTime()
    {
        minutePassCounter++;

        CountDown.text = $"00:{minutesToRecording - minutePassCounter:00}";

        List<CallType> CallsInTimeZone = WordsManager.WM.RequestCall(word);

        if (CallsInTimeZone.Count == 0) Debug.Log("No Calls To show");


        // Chequeo de llamadas en ventana horaria y condicionales
        if (!CallToShow)
        {
            foreach (CallType call in CallsInTimeZone)
            {
                if (call.GetIsCatch()) continue;
                if (call.CheckForConditionals())
                {
                    CallToShow = call;
                    call.SetCached();
                    OnCallCatch?.Invoke(this, null);
                }
            }
        }
        
        if(CallToShow)
        {
            //Chequeo de Acciones que interrumpen

            if(ActionGroupManager.AGM.CheckForPhoneActionInterrupted(word)) isInterrupted = true;
        }

        //Paso de info de la llamada cacheada
        if (minutePassCounter == minutesToRecording)
        {
            TimeManager.OnMinuteChange -= CounterPassTime;
            minutePassCounter = 0;
            CountDown.text = "00:00";
            Debug.Log("CallRecordingFinish");
            if (isInterrupted) CallToShow.SetIsinterrrupted() ;
            OnCallEndRecording?.Invoke(this, CallToShow);
            CallToShow = null;
            isInterrupted = false;
        }
    }

    public void AbortCall(Component sender, object obj)
    {
        TimeManager.OnMinuteChange -= CounterPassTime;
        minutePassCounter = 0;
        CountDown.text = "00:00";
        CallToShow = null;
        isInterrupted = false;
    }


}
