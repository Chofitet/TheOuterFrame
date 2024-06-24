using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionPlanManager : MonoBehaviour
{
    [SerializeField] GameObject ActionPlanPrefab;
    [SerializeField] ButtonElement btnElement;
    [SerializeField] ProgressorManager progressor;

    private void OnEnable()
    {
        btnElement.Onclik += OnButtonClick;
    }
    private void OnDisable()
    {
        btnElement.Onclik -= OnButtonClick;
    }

    public void SetActionPlan()
    {
        if (transform.childCount != 0) Destroy(transform.GetChild(0).gameObject);
        
        GameObject AP = Instantiate(ActionPlanPrefab, transform, false);
    }

    public void SetActionInCourse(string state)
    {
        if (!progressor.IsPossibleSetASlot()) return;
        progressor.SetActionInCourse(WordSelectedInNotebook.Notebook.GetSelectedWord(), state);
    }

    void OnButtonClick()
    {
        SetActionPlan();
    }
}
