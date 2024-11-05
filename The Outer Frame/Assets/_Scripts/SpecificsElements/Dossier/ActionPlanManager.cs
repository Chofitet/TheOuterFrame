using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionPlanManager : MonoBehaviour
{
    [SerializeField] List<StateEnum> Actions = new List<StateEnum>();
    StateEnum SpecialAction;
    [SerializeField] GameObject ActionPlanPrefab;
    [SerializeField] Transform OtherParent;
    [SerializeField] Transform OriginalParent;
    [SerializeField] GameEvent OnCloneActionPlan;
    [SerializeField] GameEvent OnSendLastActionPlan;
    [SerializeField] GameEvent OnSendEndActionPlan;
    [SerializeField] GameEvent OnDisableInput;
    [SerializeField] GameEvent OnEnableInput;
   

    [Header("Second To Last Action of the Game")]
    [SerializeField] WordData SecondToLastWord;
    [SerializeField] StateEnum SecondToLastAction;
    [SerializeField] StateEnum SecondToLastIdea;
    [SerializeField] ScriptableObject condition;
    bool isSecodToLastActionDoit;

    [Header("Final Action of the Game")]
    [SerializeField] StateEnum FinalIdea;
    
    bool IsProgressorFull;
    bool isFirstTimeIdeaAdded;

    private void Start()
    {
        SetActionPlan(null, null);
    }

    public void SetActionPlan(Component sender, object obj)
    {
        if (transform.childCount != 0) Destroy(transform.GetChild(0).gameObject);
        
        GameObject AP = Instantiate(ActionPlanPrefab, transform, false);
        AP.GetComponent<ActionPlan>().Inicialization(Actions, IsProgressorFull, isFirstTimeIdeaAdded, SecondToLastWord, SecondToLastAction, SecondToLastIdea, condition, isSecodToLastActionDoit);
        transform.Rotate(Vector3.zero);
        isFirstTimeIdeaAdded = false;
      }
    public void AddAction(Component sender, object obj)
    {
        Actions.Remove(SpecialAction);
         SetActionPlan(null, null);
         isFirstTimeIdeaAdded = true;
         StateEnum NewAction = (StateEnum)obj;
         if (NewAction.GetIsDone()) return;
        SpecialAction = NewAction;
        Actions.Add(NewAction);
    }

    public void RemoveAction(Component sender, object obj)
    {
        DataFromActionPlan data = (DataFromActionPlan)obj;
        StateEnum removeAction = data.state;
        if (removeAction.GetSpecialActionWord())
        {
            Actions.Remove(removeAction);
        }
    }

    public void ProgressorSet(Component sender, object obj)
    {
        bool x = (bool)obj;
        IsProgressorFull = x;
    }

    public void TakeReport(Component sender, object obj)
    {
        IsProgressorFull = false;
    }

    public void OnStampedPlanAction(Component sender, object obj)
    {
        GameObject actionPLan = transform.GetChild(0).gameObject;
        OnCloneActionPlan?.Invoke(this, actionPLan);
        SetActionPlan(null, null);
    }

    public void AddFinalAction(Component sender, object obj)
    {
        if (isSecodToLastActionDoit) return;
        AddAction(null, SecondToLastIdea);
        OnDisableInput?.Invoke(this, null);
        Invoke("SendLastActionPlan", 2f);
    }

     public void AddFinalFinalAction(Component sender, object obj)
    {
        if (!isSecodToLastActionDoit) return;
        AddAction(null, FinalIdea);
        OnDisableInput?.Invoke(this, null);
        Invoke("SendEndActionPlan", 2f);
    }

    void SendLastActionPlan()
    {
        OnEnableInput?.Invoke(this,null);
        OnSendLastActionPlan?.Invoke(null, SecondToLastIdea);
    }

    void SendEndActionPlan()
    {
        OnEnableInput?.Invoke(this, null);
        OnSendEndActionPlan?.Invoke(null, "Credits");
    }

    public void SetisSecodToLastActionDoit(Component sender, object obj)
    {
        isSecodToLastActionDoit = true;
        SetActionPlan(null, null);

    }
}
