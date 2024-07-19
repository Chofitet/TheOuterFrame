using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressorModuleController : MonoBehaviour
{
    [SerializeField] SlotController slot;
    bool isFull;
    bool isAbortOpen;
    Animator anim;
    bool isReady;
    [SerializeField] GameObject AbortBTN;
    [SerializeField] GameObject YesBTN;
    [SerializeField] GameObject NoBTN;
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
            if (sender.gameObject == AbortBTN)
            {
                anim.SetTrigger("abortPush");
                isAbortOpen = true;
            }
        }
        else
        {
            if(sender.gameObject == YesBTN)
            {
                anim.SetTrigger("yesPush");
                Invoke("AbortAction", 0.3f);
                isAbortOpen = false;
                isFull = false;
            }

            if (sender.gameObject == AbortBTN || sender.gameObject == NoBTN)
            {
                anim.SetTrigger("noPush");
                isAbortOpen = false;
            }
        }

    }

    public void EndAction(Component sender, object obj)
    {
        if(sender.gameObject == slot.gameObject)
        {
            anim.SetTrigger("receiveMessage");
            isFull = false;

            if(isAbortOpen)
            {
                anim.SetTrigger("noPush");
                isAbortOpen = false;
            }
        }
    }
    
    public void ReportTaked(Component sender, object obj)
    {
        GameObject report = (GameObject)obj;

        if(report == slot.gameObject)
        {
            slot.CleanSlot();
        }
    }

    public bool GetIsFull() { return isFull; }
    
    void AbortAction()
    {
        slot.AbortAction();
        anim.SetTrigger("receiveMessage");
    }
}
