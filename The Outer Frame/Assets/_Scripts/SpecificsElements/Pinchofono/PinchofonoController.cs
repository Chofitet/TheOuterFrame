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
    [SerializeField] GameObject ScreenContent;
    [SerializeField] GameObject ErrorMessageContent;
    [SerializeField] GameEvent OnDialingSound;
    [SerializeField] GameEvent OnOpenPhonePadSound;
    [SerializeField] GameEvent OnClosePhonePadSound;
    bool isRecording;
    bool IsInView;
    bool hasNumberEnter;
    bool printOnce;
    bool haveCallToPrint;

    CallType CallToPrint;
    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetFloat("tapeSpinSpeed", 0);
    }

    public void RecBTNPressed(Component sender, object obj)
    {
        anim.SetTrigger("recordPush");
        StopAllCoroutines();

        if (CallToPrint)
        {
            StartCoroutine(ShowErrorMessagePanel("You have a pending call"));
            return;
        }

        if (!hasNumberEnter)
        {
            SetIsRecordingFalse();
            StartCoroutine(ShowErrorMessagePanel("Enter a number"));
        }
        else
        {
            ShowPanel(ScreenContent);
            anim.SetBool("isCallPossible", true);
            OnStartRecording?.Invoke(this, null);
            OnClosePhonePadSound?.Invoke(this, null);
            SetIsRecordingTrue();
        }
    }

    public void PrintBTNPressed(Component sender, object obj)
    {
        anim.SetTrigger("printPush");
        StopAllCoroutines();

        if (!haveCallToPrint)
        {
            StartCoroutine(ShowErrorMessagePanel("No calls to print yet"));
        }
        else if(printOnce)
        {
            StartCoroutine(ShowErrorMessagePanel("Take printed call first"));
        }
        else
        {
            ShowPanel(ScreenContent);
            OnPrintCall?.Invoke(this, null);
            
        }
        
    }

    public void AbortBTNPressed(Component sender, object obj)
    {
        anim.SetTrigger("abortPush");
        StopAllCoroutines();

        if (CallToPrint)
        {
            StartCoroutine(ShowErrorMessagePanel("You have a pending call"));
            return;
        }

        if (!isRecording)
        {
            StartCoroutine(ShowErrorMessagePanel("No recording to abort"));
        }
        else
        {
            
            StartCoroutine(ShowErrorMessagePanel("Are you sure you want abort the recording?", 7f));
            AbortConfirmationPanel.SetActive(true);
        }

    }

    public void ConfirmAbort()
    {
        ResetAll(null,null);
        OnAbortCallRecording?.Invoke(this, null);
        OnOpenPhonePadSound?.Invoke(this, null);
    }

    public void CancelAbort()
    {
        StopAllCoroutines();
        ShowPanel(ScreenContent);
        AbortConfirmationPanel.SetActive(false);
    }

    IEnumerator ShowErrorMessagePanel(string message, float timeToAwait = 3)
    {
        ShowPanel(ErrorMessageContent);
        txtMessage.text = message;
        yield return new WaitForSeconds(timeToAwait);
        txtMessage.text = "";
        ShowPanel(ScreenContent);
    }

    void ShowPanel(GameObject panel)
    {
        if(panel == ScreenContent)
        {
            ScreenContent.SetActive(true);
            ErrorMessageContent.SetActive(false);
        }
        else
        {
            ScreenContent.SetActive(false);
            ErrorMessageContent.SetActive(true);
        }
    }

    //OnCallEndRecording
    public void SetCallToPrint(Component sender, object obj)
    {
        CallType call = (CallType)obj;
   
        CallToPrint = call;

        haveCallToPrint = true;
        SetIsRecordingFalse();
    }

    //CallToPrint
    public void PrintCall(Component sender, object obj)
    {
        if (printOnce) return;
        GameObject aux = Instantiate(CallTranscriptionPrefab, InstanciateSpot);
        aux.GetComponent<TranscriptionCallController>().Inicialization(CallToPrint);
        
        printOnce = true;

        anim.SetBool("isCallPossible", false);
        anim.SetTrigger("recordReady");
    }

    //OnSelectedWordInNotebook
    public void EnterName(Component sender, object obj)
    {
        StopAllCoroutines();
        ScreenContent.SetActive(true);
        WordData word = (WordData)obj;
        if (!IsInView) return;
        if (!word.GetIsPhoneNumberFound()) return;
        txtNumber.text = word.GetPhoneNumber();
        hasNumberEnter = true;
        txtNumber.GetComponent<TypingAnimText>().AnimateTyping();
        anim.SetTrigger("padDial");
        OnDialingSound?.Invoke(this, null);
        anim.SetTrigger("recordReady");
        anim.SetTrigger("recordReadyWobble");
    }

    //OnViewStateChange
    public void CheckPinchofonoView(Component sender, object obj)
    {
        if(!isRecording) txtNumber.text = "";
        ViewStates view = (ViewStates)obj;

        if(view != ViewStates.PinchofonoView && IsInView )
        {
            anim.SetTrigger("padClose");
            if(!isRecording && view != ViewStates.OnTakenPaperView) OnClosePhonePadSound?.Invoke(this, null);
            AbortConfirmationPanel.SetActive(false);
            hasNumberEnter = false;
        }

        IsInView = (view == ViewStates.PinchofonoView) ? true : false;

        if (IsInView && !isRecording && !haveCallToPrint)
        {
            anim.SetTrigger("padOpen");
            OnOpenPhonePadSound?.Invoke(this, null);
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
        StopAllCoroutines();
        ShowPanel(ScreenContent);
        anim.SetBool("IsRecording", false);
        anim.SetFloat("tapeSpinSpeed", 0);
        anim.SetBool("isCallPossible", false);
        anim.SetTrigger("recordReady");
        isRecording = false;
        txtNumber.text = "";
        CallToPrint = null;
        txtMessage.text = "";
        AbortConfirmationPanel.SetActive(false);
        printOnce = false;
        haveCallToPrint = false;
    }

}
