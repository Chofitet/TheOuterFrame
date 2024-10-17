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
        AP.GetComponent<ActionPlan>().Inicialization(Actions, IsProgressorFull, isFirstTimeIdeaAdded);
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

    public void ReparentToMoveActionPlan(Component sender, object obj)
    {
        
       // actionPLan.SetActive(false);
        /*transform.SetParent(OtherParent);*/
    }
    public void ReparentToActionPlan(Component sender, object obj)
    {
       /* transform.SetParent(OriginalParent);
        transform.Translate(Vector3.zero);
        transform.Rotate(Vector3.zero);
        transform.rotation = new Quaternion(0, 0, 0, 0);*/
    }
}
