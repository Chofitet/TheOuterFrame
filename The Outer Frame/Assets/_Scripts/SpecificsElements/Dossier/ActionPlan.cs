using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActionPlan : MonoBehaviour
{
    
    [SerializeField] GameObject ActionsContainer;
    Dictionary<string,ActionRowController> ActionRows = new Dictionary<string, ActionRowController>();
    string state;

    public void RegisterRow(string _name, ActionRowController script)
    {
        ActionRows.Add(_name, script);
        Debug.Log(_name);

    }

    private void Start()
    {
        DisableRows();
    }

    void DisableRows()
    {
        List<string> InactiveAgents = AgentManager.AM.GetInactiveAgents();

        if (InactiveAgents.Count == 0) return;

        for (int i = 0; i < InactiveAgents.Count; i++)
        {
            ActionRows[InactiveAgents[i]].DesactiveRow();
        }
    }


    public void WriteWordText(string _state)
    {
        state = _state;

        foreach(string row in ActionRows.Keys)
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
