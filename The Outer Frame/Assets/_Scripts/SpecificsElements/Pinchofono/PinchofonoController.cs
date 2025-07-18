using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

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
    [SerializeField] GameObject LeftRecordingNumberPanel;
    [SerializeField] GameEvent OnDialingSound;
    [SerializeField] GameEvent OnOpenPhonePadSound;
    [SerializeField] GameEvent OnClosePhonePadSound;
    [SerializeField] GameEvent OnBlinkPhoneScreen;
    [SerializeField] Canvas canvas;
    [SerializeField] GameEvent OnRefreshPinchofonoScreen;
    [SerializeField] GameObject[] Screens;
    int lastScreenNum = 0;
    WordData ActualWord;
    bool IsInView;
    bool transcriptionInQueue;

    CallType CallToPrint;
    private Animator anim;

    PhoneState currentState = PhoneState.waitingNumber;

    private void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetFloat("tapeSpinSpeed", 0);
    }

    public void ChangePhoneState(Component sender, object obj)
    {
        PhoneState nextState;
        if (obj == null) nextState = PhoneState.waitingNumber;
        else nextState = (PhoneState)obj;

        switch (nextState)
        {
            case PhoneState.waitingNumber:
                ShowPanel(0);
                break;
            case PhoneState.waitingRec:
                ShowPanel(1);
                break;
            case PhoneState.recording:
                ShowPanel(2);
                break;
            case PhoneState.waitingPrinting:
                ShowPanel(5);
                break;
        }
        currentState = nextState;

    }

    #region DialingLogic

    //OnSelectedWordInNotebook
    public void EnterName(Component sender, object obj)
    {
        if (currentState != PhoneState.waitingNumber && currentState != PhoneState.waitingRec) return;

        StopAllCoroutines();
        WordData word = (WordData)obj;
        if (word.GetPhoneNumber() == "UNLISTED") return;
        if (!IsInView) return;
        ChangePhoneState(null, PhoneState.waitingRec);
        if (!word.GetIsPhoneNumberFound())
        {
            return;
        }
        txtNumber.text = word.GetPhoneNumber();
        LeftRecordingNumberPanel.transform.GetChild(0).GetComponent<TMP_Text>().text = word.GetPhoneNumber();
        ActualWord = word;

        //anim.SetTrigger("padDial");
        StartCoroutine(AnimPadDial(word.GetPhoneNumber()));
        OnDialingSound?.Invoke(this, null);
        anim.SetTrigger("recordReady");
        anim.SetTrigger("recordReadyWobble");
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
    #endregion

    #region RecordingLogic
    public void RecBTNPressed(Component sender, object obj)
    {
        anim.SetTrigger("recordPush");
        StopAllCoroutines();

        if (currentState == PhoneState.waitingNumber) ShowPanel(3, "TO START WIRETAPPING \n ENTER A VALID PHONE NUMBER");
        else if (currentState == PhoneState.waitingRec)
        {
            anim.SetBool("isCallPossible", true);
            OnStartRecording?.Invoke(this, null);
            OnClosePhonePadSound?.Invoke(this, null);
            SetIsRecordingTrue();
            StopCoroutine(AnimPadDial(""));

            ChangePhoneState(null, PhoneState.recording);
        }
    }

    //OnStartRecordingCall
    public void SetIsRecordingTrue()
    {
        anim.SetBool("IsRecording", true);
        anim.SetFloat("tapeSpinSpeed", 1);
    }

    //OnStartRecordingCall
    public void SetIsRecordingFalse()
    {
        anim.SetBool("IsRecording", false);
        anim.SetFloat("tapeSpinSpeed", 0);
    }
    #endregion

    #region PrintLogic
    public void PrintBTNPressed(Component sender, object obj)
    {
        anim.SetTrigger("printPush");
        StopAllCoroutines();

        if (currentState == PhoneState.waitingNumber) ShowPanel(3, "TRANSCRIPT QUEUE EMPTY");
        else if (currentState == PhoneState.waitingRec) ShowPanel(3, "TRANSCRIPT QUEUE EMPTY");
        else if (currentState == PhoneState.recording) ShowPanel(3, "NOT YET, \n WIRETAPPING IN PROGRESS");
        else if (currentState == PhoneState.waitingPrinting)
        {
            if(transcriptionInQueue)
            {
                ShowPanel(3, "EMPTY THE PRINTING TRAY \n PLEASE");
                return;
            }

            ShowPanel(0);
            OnPrintCall?.Invoke(this, null);
            CallToPrint = null;
            anim.SetTrigger("padOpen");

            ChangePhoneState(null, PhoneState.waitingNumber);
        }
    }

    //OnCallEndRecording
    public void SetCallToPrint(Component sender, object obj)
    {
        CallType call = (CallType)obj;

        CallToPrint = call;

        SetIsRecordingFalse();


        StopAllCoroutines();
        ChangePhoneState(null, PhoneState.waitingPrinting);

    }

    //CallToPrint
    public void PrintCall(Component sender, object obj)
    {
        GameObject aux = Instantiate(CallTranscriptionPrefab, InstanciateSpot);
        aux.GetComponent<TranscriptionCallController>().Inicialization(CallToPrint, ActualWord);

        ActualWord = null;
        anim.SetBool("isCallPossible", false);
        anim.SetTrigger("recordReady");
        transcriptionInQueue = true;
    }
    #endregion

    #region AbortLogic
    public void AbortBTNPressed(Component sender, object obj)
    {
        canvas.enabled = true;
        anim.SetTrigger("abortPush");
        StopAllCoroutines();
        StartCoroutine(RefreshScreen());

        if (currentState == PhoneState.waitingNumber) ShowPanel(3, "THERE'S NOTHING TO CANCEL");
        else if (currentState == PhoneState.waitingRec) ShowPanel(3, "THERE'S NOTHING TO CANCEL");
        else if (currentState == PhoneState.recording) ShowPanel(4,"");
        else if (currentState == PhoneState.waitingPrinting)ShowPanel(3, "THERE'S NOTHING TO CANCEL");
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
        ShowPanel(2);
    }
    #endregion

    #region ShowPanelsLogic

    public void ShowPanel(int panelNum, string message = "")
    {
        StopAllCoroutines();
        StartCoroutine(RefreshScreen());

        foreach (GameObject screen in Screens)
        {
            CanvasGroup canvasG = screen.GetComponent<CanvasGroup>();

            if (canvasG.alpha == 1)
            {
                int auxIndx = Array.IndexOf(Screens, screen);
                if (auxIndx != 3 && auxIndx != 4) lastScreenNum = auxIndx;
            }

            canvasG.alpha = 0;
        }

        Screens[panelNum].GetComponent<CanvasGroup>().alpha = 1;

        if (panelNum == 3 || panelNum == 4) StartCoroutine(ShowErrorMessagePanel(message, lastScreenNum));
        if (panelNum == 5) OnBlinkPhoneScreen?.Invoke(this, null);
    }

    IEnumerator RefreshScreen()
    {
        OnRefreshPinchofonoScreen.Invoke(null, null);
        canvas.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.2f);
        canvas.gameObject.SetActive(true);
    }
    IEnumerator ShowErrorMessagePanel(string message, int lastPanel, float timeToAwait = 3)
    {
        txtMessage.text = message;
        yield return new WaitForSeconds(timeToAwait);
        ShowPanel(lastPanel);
    }

    #endregion


    //OnViewStateChange
    public void CheckPinchofonoView(Component sender, object obj)
    {
        ViewStates view = (ViewStates)obj;

        bool Recording_WaitingForPrint = currentState != PhoneState.recording && currentState != PhoneState.waitingPrinting;

        if (view != ViewStates.PinchofonoView && IsInView)
        {
            if(Recording_WaitingForPrint) anim.SetTrigger("padClose");
            if(Recording_WaitingForPrint && view != ViewStates.OnTakenPaperView ) OnClosePhonePadSound?.Invoke(this, null);
        }

        IsInView = (view == ViewStates.PinchofonoView) ? true : false;

        if (IsInView && Recording_WaitingForPrint)
        {
            anim.SetTrigger("padOpen");
            OnOpenPhonePadSound?.Invoke(this, null);
        }
        StopCoroutine(AnimPadDial(""));

        if (view == ViewStates.GeneralView && Recording_WaitingForPrint && PhoneState.waitingNumber != currentState)
        {
            ShowPanel(0);
            ChangePhoneState(null,PhoneState.waitingNumber);
        }
    }

    public void ResetAll(Component sender, object obj)
    {
        StopAllCoroutines();
        ChangePhoneState(null, PhoneState.waitingNumber);
        anim.SetBool("IsRecording", false);
        anim.SetFloat("tapeSpinSpeed", 0);
        anim.SetBool("isCallPossible", false);
        anim.SetTrigger("recordReady");
        txtNumber.text = "";
        //CallToPrint = null;
        WordSelectedInNotebook.Notebook.UnselectWord();
    }

    public void TakeTranscriptionFromTray(Component sender, object obj)
    {
        transcriptionInQueue = false;
    }

}

public enum PhoneState
{
    waitingNumber,
    waitingRec,
    recording,
    waitingPrinting
}