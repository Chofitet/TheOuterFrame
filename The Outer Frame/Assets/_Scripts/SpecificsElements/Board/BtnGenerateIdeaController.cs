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
        txtfield.GetComponent<FadeWordsEffect>().StartEffect(false);
        GetComponent<Button>().enabled = false;
        OnAddActionInPlanAction?.Invoke(this, state);
    }
}
