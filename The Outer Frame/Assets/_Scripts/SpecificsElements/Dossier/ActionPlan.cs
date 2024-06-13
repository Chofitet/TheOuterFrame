using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActionPlan : MonoBehaviour
{
    [SerializeField] ProgressorManager PM;
    [SerializeField] TMP_Text[] TxtInputsFields;
    string selectedWord;
    string state;

    public void WriteWordText(int Num)
    {
        for (int i = 0; i < TxtInputsFields.Length; i++)
        {
            TxtInputsFields[i].text = "";
        }

        state = TxtInputsFields[Num].gameObject.name;
        
        selectedWord = WordSelectedInNotebook.Notebook.GetSelectedWord();
        TxtInputsFields[Num].text = selectedWord;
    }

    public void ApprovedActionPlan()
    {
        PM.SetActionInCourse(selectedWord, state);
    }

}
