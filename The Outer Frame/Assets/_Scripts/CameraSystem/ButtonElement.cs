using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonElement : MonoBehaviour
{
    [SerializeField] GameEvent OnButtonElementClick;
    [SerializeField] ViewStates view;
    [SerializeField] float delay;
    bool isActive = true;
    [SerializeField] bool NotInactiveBTN;
    bool CancelEvent;

    private void OnMouseUpAsButton()
    {
        CancelEvent = false;
        if (!isActive) return;

        if(delay != 0)
        {
            Invoke("CallEvent", delay);

        }
        else OnButtonElementClick?.Invoke(this, view);


        if (!NotInactiveBTN) GetComponent<BoxCollider>().enabled = false;
    }

    void CallEvent()
    {
        if (CancelEvent)
        {
            CancelEvent = false;
            return;
        }
        OnButtonElementClick?.Invoke(this, view);
    }

    public void DisableEnableButton(Component sender, object obj)
    {
        ViewStates newState = (ViewStates)obj;

        if(newState != view)
        {
            GetComponent<BoxCollider>().enabled = true;
        }

    }

    public void Active(Component sender, object obj)
    {
        isActive = true;
    }

    public void Inactive(Component sender, object obj)
    {
        isActive = false;
    }

    public void SeTruetCancelEvent(Component sender, object obj)
    {
        CancelEvent = true;
    }

}

