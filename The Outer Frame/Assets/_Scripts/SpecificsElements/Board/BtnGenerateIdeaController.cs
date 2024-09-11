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

    public void Inicialization(StateEnum _State)
    {
        state = _State;
        txtfield.text = state.GetInfinitiveVerb();
    }

     public void OnAddAction()
     {
        OnAddActionInPlanAction?.Invoke(this, state);
     }

    public void InactiveIdea()
    {
        txtfield.text = "<s>" + txtfield.text + "</s>";
        GetComponent<Button>().enabled = false;
    }

    public StateEnum GetState() { return state; }
}
