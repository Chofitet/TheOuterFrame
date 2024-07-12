using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinchofonoManager : MonoBehaviour
{
    [SerializeField] int minutesToRecording;
    int minutePassCounter;
    WordData word;
    CallType CallToShow;
    [SerializeField] GameEvent OnCallEndRecording;
    [SerializeField] GameEvent OnCallCatch;

    public void SetRecording(Component sender, object obj)
    {
        Debug.Log("recording");
        TimeManager.OnMinuteChange += CounterPassTime;
        word = WordSelectedInNotebook.Notebook.GetSelectedWord();
    }

    void CounterPassTime()
    {
        minutePassCounter++;

        List<CallType> CallsInTimeZone = WordsManager.WM.RequestCall(word);

        if (CallsInTimeZone.Count == 0) Debug.Log("No Calls To show");

        foreach (CallType call in CallsInTimeZone)
        {
            if (call.GetIsCatch()) continue;
            if(call.CheckForConditionals())
            {
                CallToShow = call;
                call.SetCached();
                OnCallCatch?.Invoke(this, null);
            }
        }

        if (minutePassCounter == minutesToRecording)
        {
            TimeManager.OnMinuteChange -= CounterPassTime;
            minutePassCounter = 0;
            Debug.Log("CallRecordingFinish");
            OnCallEndRecording?.Invoke(this, CallToShow);
        }
    }

    public void AbortCall(Component sender, object obj)
    {
        TimeManager.OnMinuteChange -= CounterPassTime;
        minutePassCounter = 0;
    }


}
