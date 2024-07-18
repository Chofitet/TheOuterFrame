using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GoalRowController : MonoBehaviour
{
    [SerializeField] TMP_Text textField;
    [SerializeField] Toggle toggle;
    BoardGoals goal;

    public void Inicialization(BoardGoals _goal)
    {
        textField.text = _goal.GetDescription();
        goal = _goal;
    }

    public void CheckForComplete(Component sender, object obj)
    {
        if(goal.GetIsConditionalsToComplete())
        {
            toggle.isOn = true;
        }
    }
}
