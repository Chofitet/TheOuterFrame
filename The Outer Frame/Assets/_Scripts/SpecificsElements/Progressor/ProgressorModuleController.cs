using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressorModuleController : MonoBehaviour
{
    [SerializeField] SlotController slot;
    [SerializeField] GameEvent OnPrintReport;
    bool isFull;
    bool IsReadyToPrint;
    bool isAbortOpen;
    Animator anim;
    bool isReady;
    [SerializeField] GameObject PrintBTN;
    [SerializeField] GameObject SwitchAbortBTN;
    [SerializeField] GameObject AbortBTN;
    [SerializeField] BlinkMaterialEffect ReadyToPrintLED;
    bool isPrinterFull;
    Renderer mat;
    Color OriginalColor;
    [SerializeField] float intensity;

    private WordData word;
    private StateEnum state;
    private int time;

    private void Start()
    {
        anim = GetComponent<Animator>();
        mat = SwitchAbortBTN.transform.parent.GetComponent<Renderer>();
        OriginalColor = mat.material.GetColor("_EmissionColor");
    }

    private void Update()
    {
        //mat.material.SetColor("_EmissionColor", Color.white * intensity);
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
       
    }

    //OnSendAPTrackEnds
    public void InitSlot(Component sender, object obj)
    {
        if (!isReady) return;
        slot.initParameters(word, state);
        isReady = false;
        mat.material.SetColor("_EmissionColor", Color.white * 0.5f );
    }

    public void AbortLogic(Component sender, object obj)
    {
        if (!isFull) return;
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
                isFull = false;
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
            mat.material.SetColor("_EmissionColor", Color.white * 0f);
        }
    }

    void delayLigth()
    {
        PrintBTN.GetComponent<Collider>().enabled = true;
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

        mat.material.SetColor("_EmissionColor", Color.white * 0f);
    }

    //OnPressProgressorPrintBTN
    public void PrintReport(Component sender, object obj)
    {
        if(sender.gameObject == PrintBTN)
        {
            if (!isPrinterFull)
            {
                anim.SetTrigger("printMessage");
                ReadyToPrintLED.TurnOffLight(this, null);
                PrintBTN.GetComponent<Collider>().enabled = false;
                OnPrintReport?.Invoke(this, slot);
            }
            else
            {
                anim.SetTrigger("failMessage");
            }

        }
    }

    public void SetIsPrinterFull(Component sender, object obj)
    {
        bool x = (bool) obj;
        isPrinterFull = x;
    }




}
