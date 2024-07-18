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
    [SerializeField] GameEvent OnSetGeneralView;
    List<ActionRowController> Actions = new List<ActionRowController>();
    StateEnum state;

    public void Inicialization(List<StateEnum> ActionList)
    {
        InstantiateActionRows(ActionList);
    }

    void InstantiateActionRows(List<StateEnum> listActions)
    {
        List<Agent> Agents = AgentManager.AM.GetAgentList();

        foreach(StateEnum actions in listActions)
        {
            GameObject ActionInstantiate = Instantiate(ActionRowPrefab, ActionsContainer, false);
            ActionRowController script = ActionInstantiate.GetComponent<ActionRowController>();
            script.Initialization(actions);
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
        foreach(ActionRowController actions in Actions)
        {
            if (script != actions) actions.ResetRow();
            else state = script.GetState();
        }
    }

    public void ApprovedActionPlan()
    {
        OnApprovedActionPlan.Invoke(this,state);
        OnSetGeneralView?.Invoke(this, null);
    }

    public void DestroyActionPlan(Component sender, object obj)
    {
        Destroy(gameObject);
    }
}
