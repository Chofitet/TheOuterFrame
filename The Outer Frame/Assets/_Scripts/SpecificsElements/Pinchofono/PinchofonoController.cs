using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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
    [SerializeField] GameObject CounterContent;
    [SerializeField] GameObject WaveContent;
    [SerializeField] GameObject PressToRecordingText;
    [SerializeField] GameObject RecordingText;
    [SerializeField] GameObject ErrorMessageContent;
    [SerializeField] GameObject EnterValidPanel;
    [SerializeField] GameObject RecordingNumberPanel;
    [SerializeField] GameObject LeftRecordingNumberPanel;
    [SerializeField] GameEvent OnDialingSound;
    [SerializeField] GameEvent OnOpenPhonePadSound;
    [SerializeField] GameEvent OnClosePhonePadSound;
    [SerializeField] Canvas canvas;
    [SerializeField] GameEvent OnRefreshPinchofonoScreen;
    WordData ActualWord;
    bool isRecording;
    bool IsInView;
    bool hasNumberEnter;
    bool printOnce;
    bool haveCallToPrint;
    bool waitingForPrint;
    bool printInQueue;

    CallType CallToPrint;
    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetFloat("tapeSpinSpeed", 0);
        CounterContent.SetActive(false);
        WaveContent.SetActive(false);

        LeftRecordingNumberPanel.SetActive(false);
        RecordingNumberPanel.SetActive(true);
    }

    public void RecBTNPressed(Component sender, object obj)
    {
        canvas.enabled = true;
        anim.SetTrigger("recordPush");
        StopAllCoroutines();
        AbortConfirmationPanel.SetActive(false);
        StartCoroutine(RefreshScreen());

        if (waitingForPrint)
        {
            StartCoroutine(ShowErrorMessagePanel("PLEASE PRINT TRANSCRIPTION IN QUEUE"));
            return;
        }


        if (!hasNumberEnter)
        {
            SetIsRecordingFalse();
            StartCoroutine(ShowErrorMessagePanel("PLEASE ENTER A VALID PHONE NUMBER"));
        }
        else
        {
            ShowPanel(ScreenContent);
            anim.SetBool("isCallPossible", true);
            OnStartRecording?.Invoke(this, null);
            OnClosePhonePadSound?.Invoke(this, null);
            SetIsRecordingTrue();
            CounterContent.SetActive(true);
            WaveContent.SetActive(true);
            LeftRecordingNumberPanel.SetActive(true);
            RecordingNumberPanel.SetActive(false);
            PressToRecordingText.SetActive(false);
            RecordingText.SetActive(true);
            StopCoroutine(AnimPadDial(""));

            
        }
    }

    public void PrintBTNPressed(Component sender, object obj)
    {
        canvas.enabled = true;
        anim.SetTrigger("printPush");
        StopAllCoroutines();
        AbortConfirmationPanel.SetActive(false);
        StartCoroutine(RefreshScreen());

        if (!haveCallToPrint && !CallToPrint && isRecording)
        {
            StartCoroutine(ShowErrorMessagePanel("Please stand by, recording in progress"));
            return;
        }

        if (!haveCallToPrint && !CallToPrint)
        {
            StartCoroutine(ShowErrorMessagePanel("NO TRANSCRIPTION IN QUEUE"));
            return;
        }

        else if(printOnce)
        {
            StartCoroutine(ShowErrorMessagePanel("please empty the printing tray"));
        }

        if (printInQueue)
        {
            StartCoroutine(ShowErrorMessagePanel("please empty the printing tray"));
            return;
        }
        else
        {
            ShowPanel(ScreenContent);
            OnPrintCall?.Invoke(this, null);
            CallToPrint = null;
            anim.SetTrigger("padOpen");
            CounterContent.SetActive(false);
            WaveContent.SetActive(false);
            LeftRecordingNumberPanel.SetActive(false);
            RecordingNumberPanel.SetActive(true);
            EnterValidPanel.transform.GetChild(0).GetComponent<TMP_Text>().alignment = TextAlignmentOptions.Midline;

        }
        
    }

    public void AbortBTNPressed(Component sender, object obj)
    {
        canvas.enabled = true;
        anim.SetTrigger("abortPush");
        StopAllCoroutines();
        StartCoroutine(RefreshScreen());

        if (CallToPrint)
        {
            StartCoroutine(ShowErrorMessagePanel("TOO LATE TO CANCEL, THE RECORDING IS DONE"));
           
            return;
        }

        if (!isRecording)
        {
            StartCoroutine(ShowErrorMessagePanel("PLEASE BEGIN SOMETHING \n TO CANCEL IT"));
            
        }
        else
        {
            
            StartCoroutine(ShowErrorMessagePanel("", 7f));
            AbortConfirmationPanel.SetActive(true);
        }

    }

    public void ConfirmAbort()
    {
        ResetAll(null,null);
        OnAbortCallRecording?.Invoke(this, null);
        OnOpenPhonePadSound?.Invoke(this, null);
        StartCoroutine(RefreshScreen());
    }

    public void CancelAbort()
    {
        StopAllCoroutines();
        ShowPanel(ScreenContent);
        AbortConfirmationPanel.SetActive(false);
        StartCoroutine(RefreshScreen());
    }

    IEnumerator ShowErrorMessagePanel(string message, float timeToAwait = 3)
    {
        ShowPanel(ErrorMessageContent);
        txtMessage.text = message;
        yield return new WaitForSeconds(timeToAwait);
        txtMessage.text = "";
        ShowPanel(ScreenContent);
        StartCoroutine(RefreshScreen());
    }

    void ShowPanel(GameObject panel)
    {
        if (panel == ScreenContent)
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
        waitingForPrint = true;
        SetIsRecordingFalse();
        RecordingNumberPanel.SetActive(false);
        EnterValidPanel.SetActive(true);
        EnterValidPanel.transform.GetChild(0).GetComponent<TMP_Text>().text = "TRANSCRIPT READY \n FOR PRINTING";
        EnterValidPanel.transform.GetChild(0).GetComponent<TMP_Text>().alignment = TextAlignmentOptions.MidlineLeft;

        StopAllCoroutines();
        ShowPanel(ScreenContent);
        PressToRecordingText.SetActive(false);
        RecordingText.SetActive(false);

        LeftRecordingNumberPanel.SetActive(false);
        //StartCoroutine(BlinkText());
    }

    private IEnumerator BlinkText()
    {
        while (waitingForPrint)
        {
            canvas.enabled = !canvas.enabled;
            yield return new WaitForSeconds(0.5f);
        }
    }

    //CallToPrint
    public void PrintCall(Component sender, object obj)
    {
        if (printOnce) return;
        GameObject aux = Instantiate(CallTranscriptionPrefab, InstanciateSpot);
        aux.GetComponent<TranscriptionCallController>().Inicialization(CallToPrint, ActualWord);
        

        printOnce = true;
        ActualWord = null;
        waitingForPrint = false;
        printInQueue = true;
        anim.SetBool("isCallPossible", false);
        anim.SetTrigger("recordReady");
        EnterValidPanel.transform.GetChild(0).GetComponent<TMP_Text>().text = "TRANSCRIPT READY TO TAKE";
        StartCoroutine(RefreshScreen());
    }

    //OnSelectedWordInNotebook
    public void EnterName(Component sender, object obj)
    {
        if (isRecording) return;
        if (waitingForPrint) return;
        
        StopAllCoroutines();
        WordData word = (WordData)obj;
        if (word.GetPhoneNumber() == "UNLISTED") return;
        ShowPanel(ScreenContent);
        if (!IsInView) return;
        if (!word.GetIsPhoneNumberFound())
        {
            RecordingNumberPanel.SetActive(false);
            EnterValidPanel.SetActive(true);
            return;
        }
        RecordingNumberPanel.SetActive(true);
        EnterValidPanel.SetActive(false);
        txtNumber.text = word.GetPhoneNumber();
        LeftRecordingNumberPanel.transform.GetChild(0).GetComponent<TMP_Text>().text = word.GetPhoneNumber();
        hasNumberEnter = true;
        ActualWord = word;
        
        //anim.SetTrigger("padDial");
        StartCoroutine(AnimPadDial(word.GetPhoneNumber()));
        OnDialingSound?.Invoke(this, null);
        anim.SetTrigger("recordReady");
        anim.SetTrigger("recordReadyWobble");
    }

    //OnViewStateChange
    public void CheckPinchofonoView(Component sender, object obj)
    {
        if (!isRecording)
        {
            EnterValidPanel.SetActive(true);
            txtNumber.text = "";
            RecordingNumberPanel.SetActive(false);
        }
        ViewStates view = (ViewStates)obj;

        if(view != ViewStates.PinchofonoView && IsInView )
        {
            if(!isRecording && !waitingForPrint) anim.SetTrigger("padClose");
            if(!isRecording && view != ViewStates.OnTakenPaperView && !waitingForPrint) OnClosePhonePadSound?.Invoke(this, null);
            AbortConfirmationPanel.SetActive(false);
            ShowPanel(ScreenContent);
            hasNumberEnter = false;
        }

        IsInView = (view == ViewStates.PinchofonoView) ? true : false;

        if (IsInView && !isRecording && !haveCallToPrint)
        {
            anim.SetTrigger("padOpen");
            OnOpenPhonePadSound?.Invoke(this, null);
        }
        StopCoroutine(AnimPadDial(""));
        StopAllCoroutines();
        canvas.gameObject.SetActive(true);
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

    IEnumerator AnimPadDial(string number)
    {
        StartCoroutine(RefreshScreen());
        yield return new WaitForSeconds(0.2f);
        txtNumber.GetComponent<TypingAnimText>().AnimateTyping();
        number = Regex.Replace(number, @"[()\-\s]", "");

        string[] numbers = number.Select(c => c.ToString()).ToArray();

        foreach (string digit in numbers)
        {
            anim.SetTrigger("dial" + digit);
            yield return new WaitForSeconds(0.175f);
        }
    }

    IEnumerator RefreshScreen()
    {
        OnRefreshPinchofonoScreen.Invoke(null, null);
        canvas.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.2f);
        canvas.gameObject.SetActive(true);
    }

    public void ResetAll(Component sender, object obj)
    {
        StopAllCoroutines();
        ShowPanel(ScreenContent);
        RecordingNumberPanel.SetActive(false);
        EnterValidPanel.SetActive(true);
        EnterValidPanel.transform.GetChild(0).GetComponent<TMP_Text>().text = "ENTER A PHONE NUMBER \n TO WIRETAP";
        if(waitingForPrint) EnterValidPanel.transform.GetChild(0).GetComponent<TMP_Text>().text = "ENTER A PHONE NUMBER \n TO WIRETAP";
        anim.SetBool("IsRecording", false);
        anim.SetFloat("tapeSpinSpeed", 0);
        anim.SetBool("isCallPossible", false);
        anim.SetTrigger("recordReady");
        isRecording = false;
        printInQueue = false;
        txtNumber.text = "";
        //CallToPrint = null;
        txtMessage.text = "";
        AbortConfirmationPanel.SetActive(false);
        printOnce = false;
        haveCallToPrint = false;
        PressToRecordingText.SetActive(true);
        RecordingText.SetActive(false);
        CounterContent.SetActive(false);
        WaveContent.SetActive(false);

        LeftRecordingNumberPanel.SetActive(false);
        RecordingNumberPanel.SetActive(false);
    }

}
