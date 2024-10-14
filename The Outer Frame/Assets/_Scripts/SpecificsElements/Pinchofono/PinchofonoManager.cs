using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PinchofonoManager : MonoBehaviour
{
    [SerializeField] int minutesToRecording;
    [SerializeField] TMP_Text CountDown;
    int minutePassCounter;
    int SecondPassCounter = 0;
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
        TimeManager.OnSecondsChange += SecondPass;
        CountDown.text = minutesToRecording + "00";
        word = WordSelectedInNotebook.Notebook.GetSelectedWord();
    }

    void CounterPassTime()
    {
        minutePassCounter++;
        SecondPassCounter = 0;
        //CountDown.text = $"{minutesToRecording - minutePassCounter:00}";

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
                    call.SetCached(true);
                    OnCallCatch?.Invoke(this, null);
                }
            }
        }
        
        if(CallToShow)
        {
            //Chequeo de Acciones que interrumpen

            if(ActionGroupManager.AGM.CheckForPhoneActionInterrupted(word)) isInterrupted = true;

            List<WordData> involvedWords = CallToShow.GetInvolved();

            foreach(WordData w in involvedWords)
            {
                if (ActionGroupManager.AGM.CheckForPhoneActionInterrupted(w))
                {
                    isInterrupted = true;
                }
            }
        }

        //Paso de info de la llamada cacheada
        if (minutePassCounter - 1 == minutesToRecording)
        {
            TimeManager.OnMinuteChange -= CounterPassTime;
            TimeManager.OnSecondsChange -= SecondPass;
            minutePassCounter = 0;
            SecondPassCounter = 0;
            CountDown.text = "00:00:00";
            Debug.Log("CallRecordingFinish");
            if (isInterrupted) CallToShow.SetIsinterrrupted() ;
            OnCallEndRecording?.Invoke(this, CallToShow);
            CallToShow = null;
            isInterrupted = false;
        }
    }

    void SecondPass()
    {
        SecondPassCounter++;

        CountDown.text = "00:" + $"{minutesToRecording - minutePassCounter:00}:{Mathf.Abs(59 - SecondPassCounter):00}";
    }

    public void AbortCall(Component sender, object obj)
    {
        TimeManager.OnMinuteChange -= CounterPassTime;
        TimeManager.OnSecondsChange -= SecondPass;
        minutePassCounter = 0;
        CountDown.text = "00:00:00";
        CallToShow.SetCached(false);
        CallToShow = null;
        isInterrupted = false;
    }


}
