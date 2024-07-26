using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonElement : MonoBehaviour
{
    [SerializeField] GameEvent OnButtonElementClick;
    [SerializeField] ViewStates view;

    private void OnMouseUpAsButton()
    {
        OnButtonElementClick?.Invoke(this, view);
    }

    public void DisableEnableButton(Component sender, object view)
    {
      /*  ViewStates newState = (ViewStates)view;

        if(newState == ViewStates.GeneralView)
        {
            GetComponent<BoxCollider>().enabled = true;
        }
        else GetComponent<BoxCollider>().enabled = false;*/

    }
}

