using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PinchofonoController : MonoBehaviour
{
    [SerializeField] GameObject CallTranscriptionPrefab;
    [SerializeField] Transform InstanciateSpot;
    [SerializeField] TMP_Text RecordingScreen;
    bool isRecording;
    bool IsInView;

    CallType CallToPrint;
    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    //OnCallEndRecording
    public void SetCallToPrint(Component sender, object obj)
    {
        CallType call = (CallType)obj;
   
        CallToPrint = call;
    }

    //OnPressBTNPrint
    public void PrintCall(Component sender, object obj)
    {
        GameObject aux = Instantiate(CallTranscriptionPrefab, InstanciateSpot);
        aux.GetComponent<TranscriptionCallController>().Inicialization(CallToPrint);
    }

    //OnSelectedWordInNotebook
    public void EnterName(Component sender, object obj)
    {
        WordData word = (WordData)obj;
        if (!IsInView) return;
        RecordingScreen.text = word.GetPhoneNumber();
        RecordingScreen.GetComponent<TypingAnimText>().AnimateTyping();
        anim.SetTrigger("padDial");
        anim.SetTrigger("recordReady");
        anim.SetTrigger("recordReadyWobble");
    }

    //OnViewStateChange
    public void CheckPinchofonoView(Component sender, object obj)
    {
        if(!isRecording)RecordingScreen.text = "";
        ViewStates view = (ViewStates)obj;

        if(view != ViewStates.PinchofonoView && IsInView)
        {
            anim.SetTrigger("padClose");
        }

        IsInView = (view == ViewStates.PinchofonoView) ? true : false;

        if (IsInView)
        {
            anim.SetTrigger("padOpen");
        }
    }

    //OnStartRecordingCall
    public void SetIsRecordingTrue(Component sender, object obj)
    {
        anim.SetBool("IsRecording", true);
        isRecording = true;
    }

    //OnStartRecordingCall
    public void SetIsRecordingFalse(Component sender, object obj)
    {
        anim.SetBool("IsRecording", false);
        isRecording = false;
    }

    public void ResetAll(Component sender, object obj)
    {
        anim.SetBool("IsRecording", false);
        isRecording = false;
        RecordingScreen.text = "";
        CallToPrint = null;
    }

}
