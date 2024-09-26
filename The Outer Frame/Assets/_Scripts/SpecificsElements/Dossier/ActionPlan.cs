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
        else if (!state.GetSpecialActionWord() && WordSelectedInNotebook.Notebook.GetSelectedWord())
        {
            ApproveBtn.enabled = true;
            OnWriteShakeDossier?.Invoke(this, 0.5f);
        }
        else ApproveBtn.enabled = false;
    }

    public void SelectedWord(Component sender, object obj)
    {
        if(state) ApproveBtn.enabled = true;

        if(!isOneToggleSelected)
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
        OnApprovedActionPlan.Invoke(this,state);
        OnSetGeneralView?.Invoke(this, null);
    }

    public void DestroyActionPlan(Component sender, object obj)
    {
        Destroy(gameObject);
    }
}
