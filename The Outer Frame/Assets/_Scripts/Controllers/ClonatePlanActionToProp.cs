using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClonatePlanActionToProp : MonoBehaviour
{
    GameObject APCloned;
    public void ClonateActionPlan(Component sender, object obj)
    {
        GameObject AP = (GameObject)obj;

        APCloned = Instantiate(AP);

        APCloned.transform.SetParent(gameObject.transform, false);

        APCloned.transform.localPosition = Vector3.zero;
        APCloned.transform.rotation = new Quaternion(0, 0, 0, 0);
        Invoke("DeleteAP", 1);
    }

    void DeleteAP()
    {
        Destroy(APCloned);
        
    }

}
