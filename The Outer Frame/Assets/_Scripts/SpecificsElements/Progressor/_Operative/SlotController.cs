using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using TMPro.Examples;
using DG.Tweening;
using System.Linq;
using System.Xml.Linq;

public class SlotController : MonoBehaviour
{
    [SerializeField] TMP_Text Wordtxt;
    [SerializeField] TMP_Text Actiontxt;
    [SerializeField] Slider ProgressBar;
    [SerializeField] GameObject AbortIcon;
    [SerializeField] GameObject CheckIcon;
    [SerializeField] GameObject AgentIcon;
    [SerializeField] GameEvent OnFinishActionProgress;
    [SerializeField] GameEvent OnReactiveIdeaPosit;
    [SerializeField] GameObject TryAbortPanel;
    [SerializeField] GameObject LedPanel;
    [SerializeField] GameEvent OnSetCandy;
    [SerializeField] GameEvent OnSetVilifyAction;
    [SerializeField] StateEnum VilifyState;
    [SerializeField] GameEvent OnAddEntryLog;
    [SerializeField] GameEvent OnResetProgressorSlots;
    [SerializeField] GameEvent OnRecoveryAgent;
    TimeData IsVilifyLocked = new TimeData(0,0,0);

    public Action<bool> OnSetAction;

    [SerializeField] Image[] LEDObjects;
    int actionDuration;
    int secondProgress;
    WordData _word;
    StateEnum _state;
    bool isActionComplete;
    bool isAborted;
    bool isAlreadyDone;
    bool isAutomaticAction;
    bool isTheSameAction;
    bool isAVilifyBlockedAction;
    bool isAlreadyImposible;
    bool noComplete;
    int AgentsUsed = 1;
    bool isAMultiAction;

    bool isRecoveryAgent;
    StateEnum isOtherGroupActionDoing;

    TimeData timeComplete;
    bool inFillFast;
    ReportType Report;
    bool isAgentDead;
    ObjectToPrint objectType;
    Color OriginalTxtColor;
    public void initParameters(WordData word, StateEnum state, int multiActionNum, int _AgentsUsed = 1)
    {
        gameObject.SetActive(true);
        transform.GetChild(0).gameObject.SetActive(true);
        AgentIcon.SetActive(false);
        _word = word;
        Report = WordsManager.WM.RequestReport(word, state);
        _state = state;
        actionDuration = (state.GetTime() + Report.GetChangeTimeOfAction()) * 60;
        objectType = Report.GetObjectToPrint();
        Wordtxt.text = word.GetProgressorNameVersion();
        if (state.GetSpecialActionWord())
        {
            Wordtxt.text = state.GetSpeticialActionWordName();
        }
        Actiontxt.text = state.GetActioningVerb();
        if (state.GetSpecialActionWord()) Actiontxt.text = state.GetIdeaVerb();
        if (multiActionNum != 1) Actiontxt.text = "Helping " + state.GetIdeaInfinitiveVerb();
        Wordtxt.GetComponent<FontSizeAdjustToOneLine>().AdjustFontSize();
        Actiontxt.GetComponent<FontSizeAdjustToOneLine>().AdjustFontSize();
        StartCoroutine(DelayedWarp());

        isActionComplete = false;
        isAborted = false;
        isAlreadyDone = false;

        secondProgress = 0;
        ProgressBar.maxValue = actionDuration;
        ProgressBar.value = 0;

        OriginalTxtColor = Wordtxt.color;
        AgentsUsed = _AgentsUsed;
        bool isMultiActionPosible = true;
        
        if (AgentsUsed < state.GetAgentsNeeded())
        {
            isMultiActionPosible = false;
        }

        if (multiActionNum != 1)
        {
            isAMultiAction = true;
        }

        
        //ya fue hecho
        if (Report.GetWasSet())
        {
            FillFast();
            noComplete = true;
            isAlreadyDone = true;
            SetLEDState(Color.red, "Red");
        }
        //Se está haciendo el mismo en este momento
        else if (word.CheckIfActionIsDoing(state) && !isAMultiAction)
        {
            FillFast();
            isTheSameAction = true;
            noComplete = true;
            SetLEDState(Color.red, "Red");
        }
        // Se está haciendo uno del mismo ActionGroup
        else if (ActionGroupManager.AGM.ChekAreInTheSameGroup(word, state) )
        {
            FillFast();
            isOtherGroupActionDoing = _word.GetDoingAction(0);
            noComplete = true;
            SetLEDState(Color.red, "Red");
        }
        // Es una acción automática
        else if (Report.GetIsAutomatic() && !isAMultiAction)
        {
            FillFast();
            isAutomaticAction = true;
            noComplete = true;
            SetLEDState(Color.red, "Red");
        }
        else if(!IsVilifyLocked.isANullTimeData() && _state == VilifyState )
        {
            FillFast();
            isAVilifyBlockedAction = true;
            noComplete = true;
            SetLEDState(Color.red, "Red");
        }
        // Es una accion que ya no es posible 
        else if(state.GetInactiveConditionals() || word.GetInactiveState() )
        {
            FillFast();
            isAlreadyImposible = true;
            noComplete = true;
            SetLEDState(Color.red,"Red");
        }
        else if (multiActionNum != 1 && isMultiActionPosible)
        {
            TimeManager.OnSecondsChange += UpdateProgress;
            UpdateProgress();
            SetLEDState(Color.green, "Green");
        }
        else if (multiActionNum != 1 && !isMultiActionPosible)
        {
            FillFast();
            noComplete = true;
            SetLEDState(Color.red, "Red");
        }
        //no hay agentes suficientes
        else if (_state.GetAgentsNeeded() != 1 && multiActionNum == 1 && !isMultiActionPosible)
        {
            FillFast();
            noComplete = true;
            SetLEDState(Color.red, "Red");
        }
        // Es una acción válida
        else
        {
            word.SetDoingAction(state, true);
            TimeManager.OnSecondsChange += UpdateProgress;
            UpdateProgress();
            SetLEDState(Color.green, "Green");
            
            if(state == VilifyState) OnSetVilifyAction?.Invoke(this, true);
        }
       

        OnSetAction?.Invoke(noComplete);
    }

    private void OnDisable()
    {
        TimeManager.OnSecondsChange -= UpdateProgress;
    }

    void FillFast()
    {
        inFillFast = true;
        ProgressBar.maxValue = 1.5f;

    }

    private Tween progressTween;

    void UpdateProgress()
    {
        if (isActionComplete) return;

        secondProgress += 1;

        ProgressBar.value = secondProgress;

        if (secondProgress > actionDuration)
        {
            isActionComplete = true;
            if (isRecoveryAgent)
            {
                isRecoveryAgent = false;
                ResetRecovery();
                return;
            }
            if (isAlreadyDone)
            {
                if (!isAMultiAction) AutomaticAction();
            }
            else
            {
                if(!isAMultiAction) CompleteAction();
            }

            if (isAMultiAction)
            {
                CompleteMultiAction();
            }
            
        }
    }

    private void Update()
    {
        if (inFillFast && ProgressBar.value <= ProgressBar.maxValue)
        {
            ProgressBar.value += Time.deltaTime * TimeManager.timeManager.GetActuaTimeVariationSpeed() * 0.15f;
        }

        if (inFillFast && ProgressBar.value == ProgressBar.maxValue && noComplete)
        {
            if (!isAMultiAction) AutomaticAction();
            else CompleteMultiAction();
        }

    }

    private void CompleteAction()
    {
        
        _word.SetDoingAction(_state, false);
        inFillFast = false;
       
        Report = WordsManager.WM.RequestReport(_word, _state);
        if(Report.GetObjectToPrint() == ObjectToPrint.Candy1 || Report.GetObjectToPrint() == ObjectToPrint.Candy2) OnSetCandy?.Invoke(this, Report.GetObjectToPrint());
        AgentIcon.SetActive(false);
        if (!Report.GetWasSet())
        {
            isAlreadyDone = false;
            WordsManager.WM.RequestChangeState(_word, Report);
            Report.SetWasSet();
        }
        else isAlreadyDone = true;
        _state = WordsManager.WM.GetHistory(_word).Last();
        OnAddEntryLog?.Invoke(this, new LogEntryData(_word, _state.GetActionedVerb(), Report, null));
        Report.SetTimeWhenWasDone();
        timeComplete = TimeManager.timeManager.GetTime();
        OnFinishActionProgress?.Invoke(this, this);
        if (_state.GetSpecialActionWord()) _state.SetIsDone(true);
        CheckIcon.SetActive(true);

        if (Report.GetAction().name == "TheCabinInspectedFullState")
        {
            EnableAgent();
        }
    }

    public void CompleteMultiAction()
    {
        inFillFast = false;
        AgentIcon.SetActive(false);
        OnFinishActionProgress?.Invoke(this, this);
        //timeComplete = TimeManager.timeManager.GetTime();
        CheckIcon.SetActive(true);
        OnResetProgressorSlots?.Invoke(this, this.gameObject);
        ResetSlot();
    }


    void AutomaticAction()
    {
        inFillFast = false;
        OnFinishActionProgress?.Invoke(this, this);
        AgentIcon.SetActive(false);
        TimeManager.OnMinuteChange -= UpdateProgress;
        timeComplete = TimeManager.timeManager.GetTime();
    }

    public void AbortAction()
    {
        _word.SetDoingAction(_state, false);
        isAborted = true;
        inFillFast = false;
        OnFinishActionProgress?.Invoke(this, this);
        OnReactiveIdeaPosit?.Invoke(this, _state);
        TimeManager.OnSecondsChange -= UpdateProgress;
        timeComplete = TimeManager.timeManager.GetTime();
        if (_state == VilifyState) OnSetVilifyAction?.Invoke(this, false);
        AbortIcon.SetActive(true);
    }

    public void CleanSlot()
    {
        ResetSlot();
    }

    void ResetSlot()
    {
        if (isRecoveryAgent) return;
        isOtherGroupActionDoing = null;
        isTheSameAction = false;


        isAborted = false;
        isAlreadyDone = false;
        isAutomaticAction = false;
        isTheSameAction = false;
        isAVilifyBlockedAction = false;
        isAlreadyImposible = false;
        noComplete = false;
        AgentsUsed = 1;
        

        AbortIcon.SetActive(false);
        CheckIcon.SetActive(false);
        AgentIcon.SetActive(true);


        ProgressBar.value = 0;
        TimeManager.OnSecondsChange -= UpdateProgress;

        if (Report != null) if (Report.GetKillAgent() && isActionComplete) DisableAgent();
        if (isAgentDead) DisableAgent();
        if (Report != null) if (Report.GetKillAgent() && Report.GetAgentRecoveryTime() != 0 && isActionComplete)
        {
            SetRecovery(Report.GetAgentRecoveryTime());
            return;
        }

        if(isAMultiAction && isActionComplete)
        {
            CheckIcon.SetActive(true);
            AgentIcon.SetActive(false);
            ProgressBar.value = ProgressBar.maxValue;
            Invoke("ResetLedMultiAction", 2);
            return;
        }

        isAMultiAction = false;
        Report = null;
        isActionComplete = false;
        inFillFast = false;

        SetLEDState(Color.green,"Green");

        transform.GetChild(0).gameObject.SetActive(false);
    }

    void SetRecovery(int agentRecoveryTime)
    {
        Wordtxt.text = "Agent";
        Actiontxt.text = "Rehabilitating";
        Wordtxt.GetComponent<FontSizeAdjustToOneLine>().AdjustFontSize();
        Actiontxt.GetComponent<FontSizeAdjustToOneLine>().AdjustFontSize();

        actionDuration = agentRecoveryTime * 60;
        secondProgress = 0;
        ProgressBar.maxValue = actionDuration;
        ProgressBar.value = 0;
        isRecoveryAgent = true;
        isActionComplete = false;
        Report = null;
        inFillFast = false;

        AbortIcon.SetActive(false);
        CheckIcon.SetActive(false);
        AgentIcon.SetActive(true);

        TimeManager.OnSecondsChange += UpdateProgress;
        UpdateProgress();


        SetLEDState(Color.white, "Red");
    }

    void SetLEDState(Color _color, string colortxt)
    {
        foreach (Image O in LEDObjects)
        {
            O.color = _color;
        }

        ApplyMaterial(Wordtxt, colortxt);
        ApplyMaterial(Actiontxt, colortxt);
    }

    void ResetLedMultiAction()
    {
        ProgressBar.value = 0;
        isAMultiAction = false;
        Report = null;
        isActionComplete = false;
        inFillFast = false;

        AbortIcon.SetActive(false);
        CheckIcon.SetActive(false);
        AgentIcon.SetActive(true);

        SetLEDState(Color.green, "Green");

        transform.GetChild(0).gameObject.SetActive(false);
    }

    void ResetRecovery()
    {
        AbortIcon.SetActive(false);
        CheckIcon.SetActive(true);
        AgentIcon.SetActive(false);

        SetLEDState(Color.white, "Green");

        Invoke("DelayRecoveryReset", 2f);

    }

    void DelayRecoveryReset()
    {
        EnableAgent();
        OnRecoveryAgent?.Invoke(this, this);
        ResetSlot();
    }

    string materialName;
    public void ApplyMaterial(TMP_Text textField, string materialLabel = "")
    {
        if (textField.text.Contains("<material=")) return;

        materialName = "\"" + textField.font.name + "Material" + materialLabel;

        materialName = materialName.Replace(" ", "");

        string newWord = "<material=" + materialName + ">" + textField.text + "</material>";

        textField.text = newWord;

    }

    public void TurnOffProgressor(Component sender, object obj)
    {
        AbortIcon.SetActive(false);
        CheckIcon.SetActive(false);
        Report = null;
        ProgressBar.value = 0;
        TimeManager.OnSecondsChange -= UpdateProgress;
        Wordtxt.text = "";
        Actiontxt.text = "";
        SetLEDState(Color.black, "Green");
        AgentIcon.SetActive(false);
    }

    void DisableAgent()
    {
        AgentIcon.GetComponent<Image>().color = Color.red;
        if (!isAgentDead) AgentIcon.GetComponent<RectTransform>().Rotate(new Vector3(0, 0, 90));
        isAgentDead = true;
    }

    void EnableAgent()
    {
        AgentIcon.GetComponent<Image>().color = Color.green;
        AgentIcon.GetComponent<RectTransform>().Rotate(new Vector3(0, 0, -90));
        isAgentDead = false;
    }

   

    public void DisanableWithFinalReport(Component sender, object obj)
    {
        if ((StateEnum)obj == _state) return;
        TimeManager.OnSecondsChange -= UpdateProgress;

        bool aux = (AgentIcon.activeSelf);

        AgentIcon.SetActive(true);
         DisableAgent();
        if (!aux) AgentIcon.SetActive(false);

    }

    bool inBlinkAbortPanel = false;
    public void cancelTryAbortBlink()
    {
        TryAbortPanel.SetActive(false);
        LedPanel.SetActive(true);
        StopCoroutine("BlinkTryAbort");
        inBlinkAbortPanel = false;
        StartCoroutine(DelayedWarp());
    }

    public void ActiveTryAbortPanel()
    {
        if (inBlinkAbortPanel) return;
        TryAbortPanel.SetActive(true);
        LedPanel.SetActive(false);
        StartCoroutine(BlinkTryAbort());
        foreach (BlinkTMPText child in TryAbortPanel.GetComponentsInChildren<BlinkTMPText>())
        {
            child.ActiveBlink(this, null);
            child.gameObject.GetComponent<WarpTextExample>().UpdateText();
        }
    }

    IEnumerator BlinkTryAbort()
    {
        inBlinkAbortPanel = true;
        yield return new WaitForSeconds(3.4f);

        foreach (BlinkTMPText child in TryAbortPanel.GetComponentsInChildren<BlinkTMPText>())
        {
            child.TurnOffLight(this, null);
        }

        yield return new WaitForSeconds(0.4f);
        inBlinkAbortPanel = false;
        TryAbortPanel.SetActive(false);
        LedPanel.SetActive(true);
        StartCoroutine(DelayedWarp());

    }

    IEnumerator DelayedWarp()
    {
        yield return null; // espera 1 frame
        if(Wordtxt.IsActive())Wordtxt.GetComponent<WarpTextExample>().UpdateText();
        if(Actiontxt.IsActive()) Actiontxt.GetComponent<WarpTextExample>().UpdateText();
    }

    public void OnSetVilifyLockedTime(Component sender, object obj)
    {
        IsVilifyLocked = (TimeData)obj;
    }

    public WordData GetWord() { return _word; }

    public StateEnum GetState() { return _state; }

    public ReportType GetReport() { return Report; }

    public bool GetIsAborted() { return isAborted; }

    public bool getisAlreadyDone() { return isAlreadyDone; }

    public bool GetIsTheSameAction() { return isTheSameAction; }

    public StateEnum GetIsOtherGroupActionDoing() { return isOtherGroupActionDoing; }

    public bool GetIsAlreadyImposible() { return isAlreadyImposible; }

    public TimeData GetTimeComplete() { return timeComplete; }

    public bool GetIsComplete() { return isActionComplete; }

    public ObjectToPrint GetObjectType() { return objectType; }

    public bool GetisAMultiAction() { return isAMultiAction; }

    public int GetUsedAgents() { return AgentsUsed; }

    public TimeData GetIsAVilifyBlockedAction() {
        if (isAVilifyBlockedAction) return IsVilifyLocked;
        else return new TimeData(0, 0, 0);    
    }
    public bool GetNoComplete() { return noComplete; }

}
