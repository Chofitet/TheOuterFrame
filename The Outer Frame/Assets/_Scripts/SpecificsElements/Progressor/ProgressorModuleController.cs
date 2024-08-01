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
    private WordData word;
    private StateEnum state;
    private int time;

    private void Start()
    {
        anim = GetComponent<Animator>();
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
        slot.initParameters(word, state, time);
        isReady = false;
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

            PrintBTN.GetComponent<Collider>().enabled = true;
            PrintBTN.GetComponent<BlinkEffect>().ActiveBlink(this, null);

            if (isAbortOpen)
            {
                anim.SetTrigger("abortSwitchOff");
                isAbortOpen = false;
            }
        }
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
    }

    //OnPressProgressorPrintBTN
    public void PrintReport(Component sender, object obj)
    {
        if(sender.gameObject == PrintBTN)
        {
            anim.SetTrigger("printMessage");
            // anim no se pudo imprimir
            PrintBTN.GetComponent<BlinkEffect>().TurnOffLigth(this, null);
            PrintBTN.GetComponent<Collider>().enabled = false;
            OnPrintReport?.Invoke(this, slot);
        }
    }

    public void FullPrinter(Component sender, object obj)
    {
        SlotController _slot = (SlotController)obj;

        if(IsReadyToPrint && slot != _slot)
        {
            PrintBTN.GetComponent<BlinkEffect>().ActiveBlink(this, null);
            PrintBTN.GetComponent<Collider>().enabled = true;
        }
    }




}
