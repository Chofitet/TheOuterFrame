using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PinchofonoController : MonoBehaviour
{
    [SerializeField] GameObject CallTranscriptionPrefab;
    [SerializeField] Transform InstanciateSpot;
    [SerializeField] TMP_Text txtNumber;
    [SerializeField] TMP_Text txtMessage;
    [SerializeField] TMP_Text txtCountDown;
    [SerializeField] GameObject AbortConfirmationPanel;
    [SerializeField] GameEvent OnStartRecording;
    [SerializeField] GameEvent OnPrintCall;
    [SerializeField] GameEvent OnAbortCallRecording;
    bool isRecording;
    bool IsInView;
    bool hasNumberEnter;

    CallType CallToPrint;
    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetFloat("tapeSpinSpeed", 0);
    }

    public void RecBTNPressed(Component sender, object obj)
    {
        txtMessage.text = "";

        if(CallToPrint)
        {
            txtMessage.text = "You have a pending call";
            return;
        }

        if (!hasNumberEnter)
        {
            SetIsRecordingFalse();
            txtMessage.text = "Enter a number";
        }
        else
        {
            OnStartRecording?.Invoke(this, null);
            SetIsRecordingTrue();
        }
    }

    public void PrintBTNPressed(Component sender, object obj)
    {
        txtMessage.text = "";
        if (!CallToPrint)
        {
            txtMessage.text = "No calls to print yet";
        }
        else
        {
            OnPrintCall?.Invoke(this, null);
        }
    }

    public void AbortBTNPressed(Component sender, object obj)
    {
        txtMessage.text = "";

        if (CallToPrint)
        {
            txtMessage.text = "You have a pending call";
            return;
        }

        if (!isRecording)
        {
            txtMessage.text = "No recording to abort";
        }
        else
        {
            txtMessage.text = "Are you sure you want abort the recording?";
            AbortConfirmationPanel.SetActive(true);
        }

    }

    public void ConfirmAbort()
    {
        OnAbortCallRecording?.Invoke(this, null);
    }

    public void CancelAbort()
    {
        AbortConfirmationPanel.SetActive(false);
        txtMessage.text = "";
    }

    //OnCallEndRecording
    public void SetCallToPrint(Component sender, object obj)
    {
        CallType call = (CallType)obj;
   
        CallToPrint = call;
    }

    //CallToPrint
    public void PrintCall(Component sender, object obj)
    {
        GameObject aux = Instantiate(CallTranscriptionPrefab, InstanciateSpot);
        aux.GetComponent<TranscriptionCallController>().Inicialization(CallToPrint);
        SetIsRecordingFalse();
    }

    //OnSelectedWordInNotebook
    public void EnterName(Component sender, object obj)
    {
        WordData word = (WordData)obj;
        if (!IsInView) return;
        if (!word.GetIsPhoneNumberFound()) return;
        txtNumber.text = word.GetPhoneNumber();
        hasNumberEnter = true;
        txtNumber.GetComponent<TypingAnimText>().AnimateTyping();
        anim.SetTrigger("padDial");
        anim.SetTrigger("recordReady");
        anim.SetTrigger("recordReadyWobble");
    }

    //OnViewStateChange
    public void CheckPinchofonoView(Component sender, object obj)
    {
        if(!isRecording) txtNumber.text = "";
        ViewStates view = (ViewStates)obj;

        if(view != ViewStates.PinchofonoView && IsInView)
        {
            anim.SetTrigger("padClose");
            txtMessage.text = "";
            AbortConfirmationPanel.SetActive(false);
            hasNumberEnter = false;
        }

        IsInView = (view == ViewStates.PinchofonoView) ? true : false;

        if (IsInView)
        {
            anim.SetTrigger("padOpen");
        }
    }

    //OnStartRecordingCall
    public void SetIsRecordingTrue()
    {
        anim.SetBool("IsRecording", true);
        anim.SetFloat("tapeSpinSpeed", 1);
        isRecording = true;
    }

    //OnStartRecordingCall
    public void SetIsRecordingFalse()
    {
        anim.SetBool("IsRecording", false);
        anim.SetFloat("tapeSpinSpeed", 0);
        isRecording = false;
    }

    public void ResetAll(Component sender, object obj)
    {
        anim.SetBool("IsRecording", false);
        anim.SetFloat("tapeSpinSpeed", 0);
        isRecording = false;
        txtNumber.text = "";
        CallToPrint = null;
        txtMessage.text = "";
        AbortConfirmationPanel.SetActive(false);
    }

}
