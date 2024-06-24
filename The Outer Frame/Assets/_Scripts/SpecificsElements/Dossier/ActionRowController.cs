using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActionRowController : MonoBehaviour
{
    TMP_Text textTofill;
    Button button;
    ActionPlan ActionPlanReference;

    private void Awake()
    {
        textTofill = transform.GetChild(1).GetComponent<TMP_Text>();
        button = transform.GetChild(2).GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);
        ActionPlanReference = GetComponentInParent<ActionPlan>();
        ActionPlanReference.RegisterRow(name, this);
    }

    void OnButtonClick()
    {
        Debug.Log(ActionPlanReference);

        ActionPlanReference.WriteWordText(name);
        textTofill.text = WordSelectedInNotebook.Notebook.GetSelectedWord();
    }

    public void PassReference(ActionPlan AP)
    {
        ActionPlanReference = AP;
    }

    public void DeletWord()
    {
        textTofill.text = "";
    }

    public void DesactiveRow()
    {
        gameObject.SetActive(false);

    }

    
}
