using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionPlanManager : MonoBehaviour
{
    [SerializeField] GameObject ActionPlanPrefab;
    [SerializeField] ButtonElement btnElement;

    private void OnEnable()
    {
        btnElement.Onclik += OnButtonClick;
    }
    private void OnDisable()
    {
        btnElement.Onclik -= OnButtonClick;
    }
    void OnButtonClick()
    {
        SetActionPlan();
    }

    public void SetActionPlan()
    {
        if (transform.childCount != 0) Destroy(transform.GetChild(0).gameObject);
        
        GameObject AP = Instantiate(ActionPlanPrefab, transform, false);
    }

}
