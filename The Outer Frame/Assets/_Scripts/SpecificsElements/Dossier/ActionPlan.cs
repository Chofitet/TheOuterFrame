using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActionPlan : MonoBehaviour
{
    [SerializeField] GameObject ActionRowPrefab;
    [SerializeField] Transform ActionsContainer;
    [SerializeField] GameEvent OnApprovedActionPlan;
    [SerializeField] GameEvent OnProgressorFull;
    [SerializeField] GameEvent OnSetGeneralView;
    [SerializeField] Button ApproveBtn;
    [SerializeField] GameEvent OnWriteShakeDossier;
    List<ActionRowController> Actions = new List<ActionRowController>();
    WordData word;
    StateEnum state;
    bool isOneToggleSelected;
    bool isProgressorFull;
    bool isFirstTimeIdeaAdded;

    public void Inicialization(List<StateEnum> ActionList, bool _progressorfull, bool _isFirstTimeIdeaAdded)
    {
        isFirstTimeIdeaAdded = _isFirstTimeIdeaAdded;
        InstantiateActionRows(ActionList);
        ApproveBtn.enabled = false;
        isProgressorFull = _progressorfull;
        
    }

    void InstantiateActionRows(List<StateEnum> listActions)
    {
        List<Agent> Agents = AgentManager.AM.GetAgentList();

        foreach(StateEnum actions in listActions)
        {
            GameObject ActionInstantiate = Instantiate(ActionRowPrefab, ActionsContainer, false);
            ActionRowController script = ActionInstantiate.GetComponent<ActionRowController>();
            script.Initialization(actions, isFirstTimeIdeaAdded);
            script.GetButton().onClick.AddListener(() => OnButtonRowPress(script));
            Actions.Add(script);


            if (!actions.GetIfIsActive())
            {
                ActionInstantiate.GetComponent<ActionRowController>().DesactiveRow();
            }
        }
    }


    void OnButtonRowPress(ActionRowController script)
    {

        if(!isOneToggleSelected)
        {
            script.OnButtonClick();
        }

        isOneToggleSelected = true;
        foreach (ActionRowController actions in Actions)
        {
            if (script != actions) actions.ResetRow();
            else state = script.GetState();
        }

        if (state.GetSpecialActionWord()) ApproveBtn.enabled = true;
        else if (!state.GetSpecialActionWord() && word)
        {
            ApproveBtn.enabled = true;
            OnWriteShakeDossier?.Invoke(this, 0.5f);
        }
        else ApproveBtn.enabled = false;
    }

    public void SelectedWord(Component sender, object obj)
    {
        if(!isInDossier) return;
        if(state) ApproveBtn.enabled = true;
        if (!WordSelectedInNotebook.Notebook.GetSelectedWord()) return;
        word = WordSelectedInNotebook.Notebook.GetSelectedWord();

        if (!isOneToggleSelected)
        {
            OnButtonRowPress(Actions[0]);
        }

        OnWriteShakeDossier?.Invoke(this, 0.5f);
    }

    public void ApprovedActionPlan()
    {
        if (isProgressorFull)
        {
            OnProgressorFull?.Invoke(this, null);
            return;
        }

        ApproveBtn.enabled = false;
        DataFromActionPlan data = new DataFromActionPlan(word, state);
        OnApprovedActionPlan.Invoke(this, data);
        OnSetGeneralView?.Invoke(this, null);
    }

    public void DestroyActionPlan(Component sender, object obj)
    {
        Destroy(gameObject);
    }

    public void Stamp(Component sender, object obj)
    {
        ApproveBtn.transform.GetChild(0).gameObject.SetActive(true);
        Invoke("OnStampAP", 0.4f);
    }

    void OnStampAP()
    {
        ApproveBtn.transform.GetChild(0).gameObject.SetActive(false);
    }

    bool isInDossier;
    public void CheckView(Component sender, object obj)
    {
        if ((ViewStates)obj == ViewStates.DossierView) isInDossier = true;
        else isInDossier = false;
    }

    public void ProgressorSetNotFull(Component sender, object obj)
    {
        isProgressorFull = false;
    }

}

public class DataFromActionPlan
{
    public WordData word;
    public StateEnum state;

    public DataFromActionPlan(WordData _word, StateEnum _state)
    {
        word = _word;
        state = _state;
    }
}
