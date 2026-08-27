using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Rendering;

public class ProgressorModuleController : MonoBehaviour
{
    [SerializeField] SlotController slot;
    [SerializeField] GameEvent OnPrintReport;
    [SerializeField] GameEvent OnTryPrintFullPrinter;
    [SerializeField] GameEvent OnDisableAgentOnSlot;
    [SerializeField] GameEvent OnRecoveryModule;
    bool isFull;
    bool IsReadyToPrint;
    bool isAbortOpen;
    Animator anim;
    bool isReady;
    int ModuleNumber;
    [SerializeField] GameObject PrintBTN;
    [SerializeField] GameObject SwitchAbortBTN;
    [SerializeField] GameObject AbortBTN;
    [SerializeField] GameObject TryAbortBTN;
    [SerializeField] BlinkMaterialEffect ReadyToPrintLED;
    [SerializeField] Light light;
    [SerializeField] Light light2;
    [SerializeField] GameObject colliderUnused;
    [SerializeField] GameEvent OnAbortMultiAction;
    [SerializeField] GameEvent OnSendGoodVilify;
    [ColorUsage(true, true)][SerializeField] Color LedRed;

    [Header("Candy Parameters")]
    [SerializeField] GameObject Candy;
    [SerializeField] GameObject messagge;

    float InitLigthIntensity;
    float InitLigthIntensity2;
    bool isPrinterFull;
    BlinkMaterialEffect blinkmaterialAbort;
    Color OriginalColor;
    bool isEndOfGame;
    bool isChargingAction;

    private WordData word;
    private StateEnum state;
    private int time;
    ObjectToPrint objectType;

    bool isWaitingForSetSlot;
    float elapsedTime;
    float adjustedDurationForSetSlot;

    int multiAgentNum = 1;
    int AgentsUsed = 1;
    bool isAborted;

    private void Start()
    {
        anim = GetComponent<Animator>();
        blinkmaterialAbort = SwitchAbortBTN.transform.parent.GetComponent<BlinkMaterialEffect>();
        InitLigthIntensity = light.intensity;
        light.enabled = true;
        light.intensity = 0;
        InitLigthIntensity2 = light2.intensity;
        light2.enabled = true;
        light2.intensity = 0;

    }

    public void init(int _ModuleNumber)
    {
        ModuleNumber = _ModuleNumber;
    }

    private void Update()
    {
        AdjustTimeToSetSlot();
    }

    public void SetAction(WordData _word,StateEnum _state,int _time, int _multiAgentNum, bool _AgentDown, int _agentsUsed = 1)
    {
        word = _word;
        state = _state;
        time = _time;
        multiAgentNum = _multiAgentNum;
        AgentsUsed = _agentsUsed;

        isReady = true;
        isFull = true;
    }

    //OnAPProgressorEnter
    public void StartAction(Component sender, object obj)
    {
        if (!isReady) return;
        anim.SetTrigger("sendMessage");
        TurnOnLightXTime(0.8f, light, InitLigthIntensity);
        TurnOnLightXTime(0.8f, light2, InitLigthIntensity2);
        float animationDuration = 1.3f;
        adjustedDurationForSetSlot = animationDuration / TimeVariation;
        elapsedTime = 0f;
        isWaitingForSetSlot = true;
        slot.OnSetAction += activeAbortButton;
        isChargingAction = true;

        if(state.name.ToLower().Contains("vili"))
        {
            if (word.GetVilifiedNew().GetIncreaseAlertLevel() < 0) OnSendGoodVilify?.Invoke(this, null);
        }
    }

    void activeAbortButton(bool x)
    {
        // espera para que no sea posible activarlo en una automática

        TryAbortBTN.GetComponent<BoxCollider>().enabled = x;
        SwitchAbortBTN.GetComponent<BoxCollider>().enabled = !x;

        slot.OnSetAction -= activeAbortButton;
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
    void TurnOnLightXTime(float waitDuration, Light light, float lightIntencity)
    {
        if (sequenceLigth != null && sequenceLigth.IsActive()) sequenceLigth.Kill();
        sequenceLigth = DOTween.Sequence();
        sequenceLigth.Append(light.DOIntensity(lightIntencity, 0.3f))
                     .AppendInterval(waitDuration)
                    .Append(light.DOIntensity(0, 0.3f))
                    .OnComplete(() =>
                    {
                        light.intensity = 0;
                    }); ;
    }

    void TurnOnLight(Light light, float lightIntencity, float waitToTurnOn = 0)
    {
        if (sequenceLigth != null && sequenceLigth.IsActive()) sequenceLigth.Kill();
        sequenceLigth = DOTween.Sequence();
        sequenceLigth.AppendInterval(waitToTurnOn)
            .Append(light.DOIntensity(lightIntencity, 0.3f));
    }

    //OnSendAPTrackEnds
    public void InitSlot(Component sender, object obj)
    {
        if (!isReady) return;
        slot.initParameters(word, state, multiAgentNum, AgentsUsed);
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
            isChargingAction = false;

            if (slot.GetisAMultiAction())
            {
                
               if(!isAborted) PrintMultiAction();
            }
            else
            {
                anim.SetTrigger("receiveMessage");
                Invoke("delayLigth", 0.3f);
                IsReadyToPrint = true;
            }


            if (isAbortOpen)
            {
                anim.SetTrigger("abortSwitchOff");
                isAbortOpen = false;
            }

            if (slot.GetNoComplete())
            {
                ReadyToPrintLED.SetSpecificColor(LedRed);
            }
            else
            {
                if(slot.GetIsAborted())
                {
                    ReadyToPrintLED.SetSpecificColor(LedRed);
                } 
                else ReadyToPrintLED.SetOtherColor(this, null);
            }

            TurnOnLight(light, InitLigthIntensity,0.3f);
            TurnOnLight(light2, InitLigthIntensity2,0.3f);
        }
    }

    public void SetCandy(Component sender, object obj)
    {
        if (sender.gameObject == slot.gameObject)
        {
            ObjectToPrint objToPrint = (ObjectToPrint)obj;
            Candy.SetActive(true);
            Candy.GetComponent<CandyStateController>().InitializeCandy(objToPrint);
            messagge.SetActive(false);
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

        if (sender is PrinterController)  slot.SetthereAreSomethigOnPrinter(false);

        if (report == slot.gameObject)
        {
            reportTaked();
        }
    }

    void reportTaked()
    {
        slot.cancelTryAbortBlink();
        colliderUnused.GetComponent<BoxCollider>().enabled = true;
        slot.CleanSlot();
        isFull = false;
        IsReadyToPrint = true;
        resetSlot();
    }

    public bool GetIsFull() { return isFull; }
    
    void AbortAction()
    {
       // if (DisableAbort) return;
        isAborted = true;
        slot.AbortAction();
        anim.SetTrigger("receiveMessage");
        

        blinkmaterialAbort.TurnOffLight(null, null);
        OnAbortMultiAction?.Invoke(this, this);
        PrintBTN.GetComponent<BoxCollider>().enabled = true;
    }

    //OnPressProgressorPrintBTN
    public void PrintReport(Component sender, object obj)
    {
        TimeManager.timeManager.NormalizeTime();
        slot.cancelTryAbortBlink();
        slot.SetthereAreSomethigOnPrinter(true);

        if (sender.gameObject == PrintBTN)
        {
            if (!isPrinterFull)
            {
                anim.SetTrigger("printMessage");
                ReadyToPrintLED.TurnOffLight(this, null);
                PrintBTN.GetComponent<BoxCollider>().enabled = false;
                OnPrintReport?.Invoke(this, slot);
                if(slot.GetReport().GetKillAgent() && slot.GetIsComplete()) OnDisableAgentOnSlot?.Invoke(this, gameObject);
                TurnOnLight(light, 0);
                TurnOnLight(light2, 0);
                Invoke("ResetDelay", 0.3f);

            }
            else
            {
                anim.SetTrigger("failMessage");
                OnTryPrintFullPrinter?.Invoke(this, null);
            }

        }
    }

    public void PrintMultiAction()
    {
        messagge.SetActive(false);
        anim.SetTrigger("printMultiAction");
        PrintBTN.GetComponent<BoxCollider>().enabled = false;
        ReadyToPrintLED.TurnOffLight(this, null);
        PrintBTN.GetComponent<BoxCollider>().enabled = false;
        if(sequenceLigth != null && sequenceLigth.IsActive()) sequenceLigth.Kill();
        TurnOnLight(light, 0);
        TurnOnLight(light2, 0);
        anim.ResetTrigger("sendMessage");
        Invoke("resetSlot", 0.5f);
        reportTaked();
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
        messagge.SetActive(true);
        anim.ResetTrigger("printMessage");
        anim.ResetTrigger("receiveMessage");
        anim.ResetTrigger("printMultiAction");
        anim.ResetTrigger("sendMessage");
        ReadyToPrintLED.TurnOffLight(null, null);
        PrintBTN.GetComponent<BoxCollider>().enabled = false;
        colliderUnused.GetComponent<BoxCollider>().enabled = true;
        blinkmaterialAbort.TurnOffLight(null, null);
        AnimatorClipInfo[] animInfo = anim.GetCurrentAnimatorClipInfo(0);
        string currentAnimName = animInfo[0].clip.name;
        if (currentAnimName != "progresor module armature|printAction") anim.SetTrigger("resetProgressor");
        DisableAbort = true;
        multiAgentNum = 0;
        isAborted = false;
        isAbortOpen = false;
        isChargingAction = false;
        TurnOnLight(light, 0);
        TurnOnLight(light2, 0);


        TryAbortBTN.GetComponent<BoxCollider>().enabled = true;
        SwitchAbortBTN.GetComponent<BoxCollider>().enabled = false;
    }
    public void OnRecoveryAgent(Component sender, object obj)
    {
        if((SlotController) obj == slot)
        {
            OnRecoveryModule?.Invoke(this, this);
        }
    }

    void ResetDelay()
    {
        Candy.SetActive(false);
        messagge.SetActive(true);
    }
    public void OnGameOver(Component sender,object obj)
    {
        TurnOnLight(light, 0);
        TurnOnLight(light2, 0);
    }

    public void TryAbortAnim(Component sender, object obj)
    {
        GameObject btn = (GameObject)obj;

        if (btn == TryAbortBTN)
        {
            anim.SetTrigger("tryAbortSwitch");
            if (isChargingAction) return;
            slot.ActiveTryAbortPanel();
        }
            
    }

    float TimeVariation = 1;
    public void accelerateAnims(Component sender, object obj)
    {
        TimeVariation = (float)obj;

        if (TimeVariation < 1 && TimeVariation != 0)
        {
            TimeVariation = 1;
        }

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

    public int GetModuleNumber() { return ModuleNumber; }
    public SlotController GetSlot() { return slot; }

}
