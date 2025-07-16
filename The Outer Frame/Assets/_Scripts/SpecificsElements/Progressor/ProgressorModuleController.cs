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
    [SerializeField] GameObject TryAbortBTN;
    [SerializeField] BlinkMaterialEffect ReadyToPrintLED;
    [SerializeField] Light light;
    [SerializeField] GameObject colliderUnused;
    float InitLigthIntensity;
    bool isPrinterFull;
    BlinkMaterialEffect blinkmaterialAbort;
    Color OriginalColor;
    bool isEndOfGame;

    private WordData word;
    private StateEnum state;
    private int time;

    bool isWaitingForSetSlot;
    float elapsedTime;
    float adjustedDurationForSetSlot;

    private void Start()
    {
        anim = GetComponent<Animator>();
        blinkmaterialAbort = SwitchAbortBTN.transform.parent.GetComponent<BlinkMaterialEffect>();
        InitLigthIntensity = light.intensity;
        light.enabled = true;
        light.intensity = 0;

    }

    private void Update()
    {
        AdjustTimeToSetSlot();
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
        TryAbortBTN.GetComponent<BoxCollider>().enabled = false;
        SwitchAbortBTN.GetComponent<BoxCollider>().enabled = true;
        TurnOnLight(0.8f);
        float animationDuration = 1.3f;
        adjustedDurationForSetSlot = animationDuration / TimeVariation;
        elapsedTime = 0f;
        isWaitingForSetSlot = true;
    }

    void AdjustTimeToSetSlot()
    {
        if (!isWaitingForSetSlot) return;

        // Incrementar el tiempo transcurrido teniendo en cuenta la variación del tiempo
        elapsedTime += Time.deltaTime * TimeVariation;

       

        if (elapsedTime >= adjustedDurationForSetSlot)
        {
            // Finalizar espera y ejecutar InitSlot
            isWaitingForSetSlot = false;
            InitSlot(null, null);
        }
    }

    Sequence sequenceLigth;
    void TurnOnLight(float waitDuration)
    {
        if (sequenceLigth != null && sequenceLigth.IsActive()) sequenceLigth.Kill();
        sequenceLigth = DOTween.Sequence();
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
        colliderUnused.GetComponent<BoxCollider>().enabled = false;
    }

    public void AbortLogic(Component sender, object obj)
    {
        if (isEndOfGame) return;
        //if (!isFull) return;
        if (!isAbortOpen)
        {
            if (sender.gameObject == SwitchAbortBTN)
            {
                anim.ResetTrigger("abortSwitchOff");
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
                anim.ResetTrigger("abortSwitchOn");
                anim.SetTrigger("abortSwitchOff");
                isAbortOpen = false;
            }

        }

    }

    public void EndAction(Component sender, object obj)
    {
        if(sender.gameObject == slot.gameObject)
        {
            TryAbortBTN.GetComponent<BoxCollider>().enabled = true;
            SwitchAbortBTN.GetComponent<BoxCollider>().enabled = false;
            slot.cancelTryAbortBlink();

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
        PrintBTN.GetComponent<BoxCollider>().enabled = true;
        blinkmaterialAbort.TurnOffLight(null, null);
        ReadyToPrintLED.ActiveBlink(this, null);
    }
    
    public void ReportTaked(Component sender, object obj)
    {
        GameObject report = (GameObject)obj;
        
        if (report == slot.gameObject)
        {
            slot.cancelTryAbortBlink();
            colliderUnused.GetComponent<BoxCollider>().enabled = true;
            slot.CleanSlot();
            isFull = false;
            IsReadyToPrint = true;
        }
    }

    public bool GetIsFull() { return isFull; }
    
    void AbortAction()
    {
        if (DisableAbort) return;
        slot.AbortAction();
        anim.SetTrigger("receiveMessage");

        blinkmaterialAbort.TurnOffLight(null, null);
    }

    //OnPressProgressorPrintBTN
    public void PrintReport(Component sender, object obj)
    {
        TimeManager.timeManager.NormalizeTime();
        slot.cancelTryAbortBlink();
        if (sender.gameObject == PrintBTN)
        {
            if (!isPrinterFull)
            {
                anim.SetTrigger("printMessage");
                ReadyToPrintLED.TurnOffLight(this, null);
                PrintBTN.GetComponent<BoxCollider>().enabled = false;
                OnPrintReport?.Invoke(this, slot);
                if(slot.GetReport().GetKillAgent() && slot.GetIsComplete()) OnDisableAgentOnSlot?.Invoke(this, gameObject);
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

    public void OnEndGame(Component sender, object obj)
    {
        isEndOfGame = true;
    }

    public StateEnum GetState() { return state; }

    public void ResetAndCleanModule(Component sender, object obj)
    {
        slot.CleanSlot();
        resetSlot();
    }

    bool DisableAbort;
    void resetSlot()
    {
        isFull = false;
        anim.ResetTrigger("printMessage");
        anim.ResetTrigger("receiveMessage");
        ReadyToPrintLED.TurnOffLight(null, null);
        PrintBTN.GetComponent<BoxCollider>().enabled = false;
        colliderUnused.GetComponent<BoxCollider>().enabled = true;
        blinkmaterialAbort.TurnOffLight(null, null);
        anim.SetTrigger("resetProgressor");
        DisableAbort = true;

    }


    public void TryAbortAnim(Component sender, object obj)
    {
        GameObject btn = (GameObject)obj;

        if (btn == TryAbortBTN)
        {
            anim.SetTrigger("tryAbortSwitch");
            slot.ActiveTryAbortPanel();
        }
            
    }

    float TimeVariation = 1;
    public void accelerateAnims(Component sender, object obj)
    {
        TimeVariation = (float)obj;
        anim.SetFloat("speed", TimeVariation);

        if(TimeVariation == 1)
        {
            adjustedDurationForSetSlot = 1.3f;
        }

        if (isWaitingForSetSlot)
        {
            if (sequenceLigth != null && sequenceLigth.IsActive()) sequenceLigth.Kill();
            light.intensity = 0;
            float remainingTime = adjustedDurationForSetSlot - elapsedTime;
            adjustedDurationForSetSlot = remainingTime / TimeVariation + elapsedTime;
        }
    }

}
