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
    [SerializeField] GameEvent OnFinalActionSend;
    [SerializeField] GameEvent OnShakeNotebook;
    [SerializeField] GameObject shakeBtn;
    WordData FinalActionWord;
    StateEnum FinalActionState;
    StateEnum FinalActionIdea;
    List<ActionRowController> Actions = new List<ActionRowController>();
    WordData word;
    StateEnum state;
    bool isOneToggleSelected;
    bool isProgressorFull;
    bool isFirstTimeIdeaAdded;
    bool isSecodToLastActionDoit;
    IConditionable condition;

    public void Inicialization(List<StateEnum> ActionList, bool _progressorfull, bool _isFirstTimeIdeaAdded, WordData _FinalActionWord, StateEnum _FinalActionState, StateEnum _FinalActionIdea,ScriptableObject _condition, bool _isSecodToLastActionDoit)
    {
        isFirstTimeIdeaAdded = _isFirstTimeIdeaAdded;
        InstantiateActionRows(ActionList);
        ApproveBtn.enabled = false;
        isProgressorFull = _progressorfull;
        FinalActionWord = _FinalActionWord;
        FinalActionState = _FinalActionState;
        FinalActionIdea = _FinalActionIdea;
        isSecodToLastActionDoit = _isSecodToLastActionDoit;
        condition = _condition as IConditionable;
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

        if (state.GetSpecialActionWord())
        {
            shakeBtn.SetActive(false);
            ApproveBtn.enabled = true;
        }
        else if (!state.GetSpecialActionWord() && word)
        {
            shakeBtn.SetActive(false);
            ApproveBtn.enabled = true;
            OnWriteShakeDossier?.Invoke(this, 0.5f);
        }
        else
        {
            shakeBtn.SetActive(true);
            ApproveBtn.enabled = false;
        }
    }

    public void SelectedWord(Component sender, object obj)
    {
        if(!isInDossier) return;
        if (state)
        {
            shakeBtn.SetActive(false);
            ApproveBtn.enabled = true;
        }
        bool isActionIdeaSelect = false;
        if (Actions.Count >= 10)
        {
            isActionIdeaSelect = Actions[9].GetIsOn();
            if (isActionIdeaSelect) isOneToggleSelected = false;
        }
        if (!WordSelectedInNotebook.Notebook.GetSelectedWord()) return;
        word = WordSelectedInNotebook.Notebook.GetSelectedWord();

        if (!isOneToggleSelected || isActionIdeaSelect)
        {
            if (Actions.Count == 0) return;
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

        if (isSecodToLastActionDoit)
        {
            OnFinalActionSend?.Invoke(this, null);
            ProgressorSetFull(null, null);
            return;
        }

        if(FinalActionWord == word && condition.GetStateCondition())
        {
            OnFinalActionSend?.Invoke(this, null);
           // state = FinalActionIdea;
            ProgressorSetFull(null, null);
            return;
        }

        SendActionToProgressor();
    }

    public void ShakeBtn()
    {
        if (!isOneToggleSelected) return;
        OnShakeNotebook?.Invoke(this, null);
    }

    void SendActionToProgressor()
    {
        ApproveBtn.enabled = false;
        shakeBtn.SetActive(true);
        DataFromActionPlan data = new DataFromActionPlan(word, state);
        OnApprovedActionPlan.Invoke(this, data);
        OnSetGeneralView?.Invoke(this, null);
    }

    public void SendFinalActionToProgressor(Component sender, object obj)
    {
        ApproveBtn.enabled = false;
        shakeBtn.SetActive(true);
        DataFromActionPlan data = new DataFromActionPlan(FinalActionWord, FinalActionIdea);
        OnApprovedActionPlan.Invoke(this, data);
        if(Actions.Count >= 9) Actions[9].CheckToggle();
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
        if ((ViewStates)obj == ViewStates.DossierView || (ViewStates)obj == ViewStates.OnTakenPaperView) isInDossier = true;
        else isInDossier = false;
    }

    public void ProgressorSetNotFull(Component sender, object obj)
    {
        isProgressorFull = false;
    }

    public void ProgressorSetFull(Component sender, object obj)
    {
        isProgressorFull = true;
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
