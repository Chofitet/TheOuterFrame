using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ProgressorModuleController : MonoBehaviour
{
    [SerializeField] SlotController slot;
    [SerializeField] GameEvent OnPrintReport;
    [SerializeField] GameEvent OnTryPrintFullPrinter;
    [SerializeField] GameEvent OnDisableAgentOnSlot;
    bool isFull;
    bool IsReadyToPrint;
    bool isAbortOpen;
    Animator anim;
    bool isReady;
    [SerializeField] GameObject PrintBTN;
    [SerializeField] GameObject SwitchAbortBTN;
    [SerializeField] GameObject AbortBTN;
    [SerializeField] BlinkMaterialEffect ReadyToPrintLED;
    [SerializeField] Light light;
    float InitLigthIntensity;
    bool isPrinterFull;
    BlinkMaterialEffect blinkmaterialAbort;
    Color OriginalColor;

    private WordData word;
    private StateEnum state;
    private int time;

    private void Start()
    {
        anim = GetComponent<Animator>();
        blinkmaterialAbort = SwitchAbortBTN.transform.parent.GetComponent<BlinkMaterialEffect>();
        InitLigthIntensity = light.intensity;
        light.enabled = true;
        light.intensity = 0;

    }

    public void SetAction(WordData _word,StateEnum _state,int _time)
    {
        word = _word;
        state = _state;
        time = _time;

        isReady = true;
        isFull = true;
    }

    //OnAPProgressorEnter
    public void StartAction(Component sender, object obj)
    {
        if (!isReady) return;
        anim.SetTrigger("sendMessage");
        TurnOnLight(0.8f);
    }

    void TurnOnLight(float waitDuration)
    {
        Sequence sequenceLigth = DOTween.Sequence();
        sequenceLigth.Append(light.DOIntensity(InitLigthIntensity, 0.3f))
                     .AppendInterval(waitDuration)
                    .Append(light.DOIntensity(0, 0.3f))
                    .OnComplete(() =>
                    {
                        light.intensity = 0;
                    }); ;
    }


    //OnSendAPTrackEnds
    public void InitSlot(Component sender, object obj)
    {
        if (!isReady) return;
        slot.initParameters(word, state);
        isReady = false;
        blinkmaterialAbort.TurnOnLigth(null, null);
    }

    public void AbortLogic(Component sender, object obj)
    {
       
        //if (!isFull) return;
        if (!isAbortOpen)
        {
            if (sender.gameObject == SwitchAbortBTN)
            {
                anim.SetTrigger("abortSwitchOn");
                isAbortOpen = true;
            }
        }
        else
        {
            if(sender.gameObject == AbortBTN)
            {
                anim.SetTrigger("abortPush");
                Invoke("AbortAction", 0.3f);
                isAbortOpen = false;
            }

            if (sender.gameObject == SwitchAbortBTN)
            {
                anim.SetTrigger("abortSwitchOff");
                isAbortOpen = false;
            }

        }

    }

    public void EndAction(Component sender, object obj)
    {
        if(sender.gameObject == slot.gameObject)
        {
            anim.SetTrigger("receiveMessage");
            IsReadyToPrint = true;
            

            Invoke("delayLigth", 0.3f);

            if (isAbortOpen)
            {
                anim.SetTrigger("abortSwitchOff");
                isAbortOpen = false;
            }
            
        }
    }

    void delayLigth()
    {
        PrintBTN.GetComponent<Collider>().enabled = true;
        blinkmaterialAbort.TurnOffLight(null, null);
        ReadyToPrintLED.ActiveBlink(this, null);
    }
    
    public void ReportTaked(Component sender, object obj)
    {
        GameObject report = (GameObject)obj;
        
        if (report == slot.gameObject)
        {
            slot.CleanSlot();
            isFull = false;
            IsReadyToPrint = true;
        }
    }

    public bool GetIsFull() { return isFull; }
    
    void AbortAction()
    {
        slot.AbortAction();
        anim.SetTrigger("receiveMessage");

        blinkmaterialAbort.TurnOffLight(null, null);
    }

    //OnPressProgressorPrintBTN
    public void PrintReport(Component sender, object obj)
    {
        TimeManager.timeManager.NormalizeTime();
        if(sender.gameObject == PrintBTN)
        {
            if (!isPrinterFull)
            {
                anim.SetTrigger("printMessage");
                ReadyToPrintLED.TurnOffLight(this, null);
                PrintBTN.GetComponent<Collider>().enabled = false;
                OnPrintReport?.Invoke(this, slot);
                if(slot.GetReport().GetKillAgent()) OnDisableAgentOnSlot?.Invoke(this, gameObject);
            }
            else
            {
                anim.SetTrigger("failMessage");
                OnTryPrintFullPrinter?.Invoke(this, null);
            }

        }
    }

    public void SetIsPrinterFull(Component sender, object obj)
    {
        bool x = (bool) obj;
        isPrinterFull = x;
    }




}
