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
    List<ActionRowController> Actions = new List<ActionRowController>();
    StateEnum state;

    private void OnEnable()
    {
        InstantiateActionRows();
    }

    void InstantiateActionRows()
    {
        List<StateEnum> Agents = AgentManager.AM.GetAgentList();

        foreach(StateEnum actions in Agents)
        {
            GameObject AgentInstantiate = Instantiate(ActionRowPrefab, ActionsContainer, false);
            ActionRowController script = AgentInstantiate.GetComponent<ActionRowController>();
            script.Initialization(actions);
            script.GetButton().onClick.AddListener(() => OnButtonRowPress(script));
            Actions.Add(script);

            
            if (!actions.GetIfIsActive())
            {
                AgentInstantiate.GetComponent<ActionRowController>().DesactiveRow();
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
        Destroy(gameObject);
    }


}
