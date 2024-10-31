using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonElement : MonoBehaviour
{
    [SerializeField] GameEvent OnButtonElementClick;
    [SerializeField] ViewStates view;
    bool isActive = true;
    [SerializeField] bool NotInactiveBTN;

    private void OnMouseUpAsButton()
    {
        if (!isActive) return;
        OnButtonElementClick?.Invoke(this, view);
        if(!NotInactiveBTN) GetComponent<BoxCollider>().enabled = false;
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
}

