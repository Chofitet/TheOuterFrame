using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionPlanManager : MonoBehaviour
{
    [SerializeField] List<StateEnum> Actions = new List<StateEnum>();
    [SerializeField] GameObject ActionPlanPrefab;
    [SerializeField] Transform OtherParent;
    [SerializeField] Transform OriginalParent;


    public void SetActionPlan(Component sender, object obj)
    {
        if (transform.childCount != 0) Destroy(transform.GetChild(0).gameObject);
        
        GameObject AP = Instantiate(ActionPlanPrefab, transform, false);
        AP.GetComponent<ActionPlan>().Inicialization(Actions);

        /*transform.position = Vector3.zero;
        
        float x = Mathf.Round(transform.position.x);
        float y = Mathf.Round(transform.position.y);
        float z = Mathf.Round(transform.position.z);

        transform.position = new Vector3(x, y, z);
        */
    }

    public void AddAction(Component sender, object obj)
    {
        StateEnum NewAction = (StateEnum)obj;
        Actions.Add(NewAction);
    }

    public void ReparentToMoveActionPlan(Component sender, object obj)
    {
        transform.SetParent(OtherParent);
    }
    public void ReparentToActionPlan(Component sender, object obj)
    {
        transform.SetParent(OriginalParent);
    }


}
