using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

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
    [SerializeField] GameEvent OnSendGoodVilify;
    [SerializeField] BoxCollider BlockStampiInput;
    WordData FinalActionWord;
    StateEnum FinalActionState;
    StateEnum FinalActionIdea;
    List<ActionRowController> Actions = new List<ActionRowController>();
    WordData word;
    StateEnum state;
    bool isOneToggleSelected;
    bool isProgressorFull;
    bool AllModulesAreTaked;
    bool isFirstTimeIdeaAdded;
    bool isSecodToLastActionDoit;
    IConditionable condition;


    public void Inicialization(List<StateEnum> ActionList, bool _progressorfull, bool _isFirstTimeIdeaAdded, WordData _FinalActionWord, StateEnum _FinalActionState, StateEnum _FinalActionIdea, ScriptableObject _condition, bool _isSecodToLastActionDoit)
    {
        isFirstTimeIdeaAdded = _isFirstTimeIdeaAdded;
        InstantiateActionRows(ActionList);
        ApproveBtn.enabled = false;
        isProgressorFull = _progressorfull;
        if (isProgressorFull) AllModulesAreTaked = true;
        FinalActionWord = _FinalActionWord;
        FinalActionState = _FinalActionState;
        FinalActionIdea = _FinalActionIdea;
        isSecodToLastActionDoit = _isSecodToLastActionDoit;
        condition = _condition as IConditionable;
    }

    void InstantiateActionRows(List<StateEnum> listActions)
    {
        foreach (StateEnum actions in listActions)
        {
            GameObject ActionInstantiate = Instantiate(ActionRowPrefab, ActionsContainer, false);
            ActionRowController script = ActionInstantiate.GetComponent<ActionRowController>();
            script.Initialization(actions, isFirstTimeIdeaAdded);
            script.GetButton().GetComponent<OnClickDownEvent>().onPointerDown.AddListener(() => OnButtonRowPress(script));
            Actions.Add(script);


            if (!actions.GetIfIsActive())
            {
                ActionInstantiate.GetComponent<ActionRowController>().DesactiveRow();
            }
        }
    }

    void OnButtonRowPress(ActionRowController script)
    {

        if (!isOneToggleSelected)
        {
            script.OnButtonClick(true);
        }

        isOneToggleSelected = true;
        bool exit = false;
        foreach (ActionRowController actions in Actions)
        {
            if (script != actions) actions.ResetRow();
            else
            {
                state = script.GetState();
                if (script.GetIsOn())
                {
                    actions.ResetRow();
                    shakeBtn.SetActive(true);
                    ApproveBtn.enabled = false;
                    exit = true;
                    isOneToggleSelected = false;

                    continue;
                }

            }
            if (exit) return;

        }

        if (state.GetSpecialActionWord())
        {
            shakeBtn.SetActive(false);
            ApproveBtn.enabled = true;

            if (!isOneToggleSelected)
            {
                shakeBtn.SetActive(true);
                ApproveBtn.enabled = false;
            }
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

    public void DisableButtonsOnWritting(Component sender, object obj)
    {
        EnableDisableAllBtns(false);
    }

    public void SelectedWord(Component sender, object obj)
    {
        EnableDisableAllBtns(true);

        if (!isInDossier) return;

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

        if (FinalActionWord == word && condition.GetStateCondition())
        {
            OnFinalActionSend?.Invoke(this, null);
            // state = FinalActionIdea;
            ProgressorSetFull(null, null);
            return;
        }

        bool isOneActionOn = false;
        foreach (ActionRowController row in Actions)
        {
            if (row.GetIsAnSpecialAction() && !row.GetIsOn()) row.ResetActionRow();

            if (row.GetIsOn()) isOneActionOn = true;
        }
        if (!isOneActionOn) return;

        SendActionToProgressor();
    }

    public void ShakeBtn()
    {
        //if (!isOneToggleSelected) return;
        OnShakeNotebook?.Invoke(this, null);
    }

    void SendActionToProgressor()
    {
        ApproveBtn.enabled = false;
        shakeBtn.SetActive(true);
        DataFromActionPlan data = new DataFromActionPlan(word, state);
        if (state.GetSpecialActionWord()) state.SetisWrittenOnAP(false);
        OnApprovedActionPlan.Invoke(this, data);
        OnSetGeneralView?.Invoke(this, null);
        if (state.GetNeedWordLocation()) word.SetIsPendingToShowLocation(false);

    }
    public void SendFinalActionToProgressor(Component sender, object obj)
    {
        ApproveBtn.enabled = false;
        shakeBtn.SetActive(true);
        DataFromActionPlan data = new DataFromActionPlan(FinalActionWord, FinalActionIdea);
        OnApprovedActionPlan.Invoke(this, data);
        Invoke("delayCheckButton", 0.02f);
        OnSetGeneralView?.Invoke(this, null);

    }



    void delayCheckButton()
    {
        if (Actions.Count >= 9) Actions[9].CheckToggle();
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
    Coroutine DisableBlockStampInputCoroutine;
    public void CheckView(Component sender, object obj)
    {
        ViewStates actualView = (ViewStates)obj;
        if (actualView == ViewStates.DossierView || actualView == ViewStates.OnTakenPaperView) isInDossier = true;
        else isInDossier = false;

        if (actualView == ViewStates.DossierView)
        {
            if (DisableBlockStampInputCoroutine != null) StopCoroutine(DisableBlockStampInputCoroutine);
            DisableBlockStampInputCoroutine = StartCoroutine(DisableBlockStampInput());
        }
        else
        {
            if (DisableBlockStampInputCoroutine != null) StopCoroutine(DisableBlockStampInputCoroutine);
            BlockStampiInput.enabled = true;
        }

    }

    IEnumerator DisableBlockStampInput()
    {
        yield return new WaitForSeconds(0.5f);
        BlockStampiInput.enabled = false;

    }

    public void ProgressorSetNotFull(Component sender, object obj)
    {
        if (obj != null)
        {
            if (obj is SlotController slotController)
            {
                if (slotController.GetthereAreSomethigOnPrinter()) return;
            }
            else if (obj is GameObject slotControllerGO)
            {
                if (slotControllerGO.gameObject.GetComponent<SlotController>().GetthereAreSomethigOnPrinter()) return;
            }
        }

        if (!AgentDownInlastReportTaked)
        {
            isProgressorFull = false;
        }
        AgentDownInlastReportTaked = false;
    }

    public void ProgressorSetFull(Component sender, object obj)
    {
        isProgressorFull = true;
    }

    bool AgentDownInlastReportTaked;

    public void TakeReport(Component sender, object obj)
    {
        GameObject report = (GameObject)obj;

        bool AgentDown = report.GetComponent<IndividualReportController>().GetRepoertype().GetKillAgent();

        bool reportCompleted = report.GetComponent<IndividualReportController>().GetCompleted();

        if (AgentDown && AllModulesAreTaked && reportCompleted)
        {
            AgentDownInlastReportTaked = true;
            return;
        };

        AllModulesAreTaked = false;
        isProgressorFull = false;
    }


    public void EnableDisableAllBtns(bool x)
    {
        foreach (ActionRowController actions in Actions)
        {
            actions.GetButton().enabled = x;
        }
    }

    public void OnEraseInactiveOrReplacedWords(Component sender, object obj)
    {
        if(obj == null)
        {
            isOneToggleSelected = false;
            return;
        }

        WordData toEraseWord = (WordData)obj;

        if(toEraseWord == word) isOneToggleSelected = false;

    }

    public void ForceChangeWordSelect(Component sender, object obj)
    {
        if (obj == null) return;
        word = (WordData) obj;
        shakeBtn.SetActive(false);
        ApproveBtn.enabled = true;
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
