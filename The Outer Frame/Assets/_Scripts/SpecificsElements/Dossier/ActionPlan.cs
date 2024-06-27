using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActionPlan : MonoBehaviour
{
    
    [SerializeField] GameObject ActionsContainer;
    Dictionary<StateEnum, ActionRowController> ActionRows = new Dictionary<StateEnum, ActionRowController>();
    StateEnum state;

    public void RegisterRow(StateEnum _name, ActionRowController script)
    {
        ActionRows.Add(_name, script);
    }

    private void Start()
    {
        DisableRows();
    }

    void DisableRows()
    {
        List<StateEnum> InactiveAgents = AgentManager.AM.GetInactiveAgents();

        if (InactiveAgents.Count == 0) return;

        for (int i = 0; i < InactiveAgents.Count; i++)
        {
            ActionRows[InactiveAgents[i]].DesactiveRow();
        }
    }


    public void WriteWordText(StateEnum _state)
    {
        state = _state;

        foreach(StateEnum row in ActionRows.Keys)
        {
            ActionRows[row].DeletWord();
        }
    }

    public void ApprovedActionPlan()
    {
        GetComponentInParent<ActionPlanManager>().SetActionInCourse(state);
        Destroy(gameObject);
    }

}
