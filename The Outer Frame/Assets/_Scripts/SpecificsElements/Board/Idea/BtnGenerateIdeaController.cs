using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BtnGenerateIdeaController : MonoBehaviour
{

    [SerializeField] GameEvent OnAddActionInPlanAction;
    [SerializeField] TMP_Text txtfield;
    StateEnum state;
    bool isInactive;

    public void Inicialization(StateEnum _State)
    {
        state = _State;
        //si ya hay algo escrito, queda eso
        if(txtfield.text == "") txtfield.text = state.GetInfinitiveVerb();
    }

     public void OnAddAction()
     {
        if (isInactive) return;
        OnAddActionInPlanAction?.Invoke(this, state);
        GetComponent<Button>().enabled = false;
        Invoke("ActiveBTN", 3f);
    }

    void ActiveBTN()
    {
        if (isInactive) return;
        GetComponent<Button>().enabled = true;
    }

    public void InactiveIdea()
    {
        txtfield.text = "<s>" + txtfield.text + "</s>";
        GetComponent<Button>().enabled = false;
        isInactive = true;
    }

    public void ActivedDesactiveIdeaBTN(bool x)
    {
        if (isInactive) return;
        GetComponent<Button>().enabled = x;
    }

    public StateEnum GetState() { return state; }
}
