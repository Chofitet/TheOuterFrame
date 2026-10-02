using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassTriggerAppersObject : MonoBehaviour
{
    [SerializeField] GameObject ObjectToAppear;
    [SerializeField] GameObject ObjectToDisappear;
    // Update is called once per frame
    void Update()
    {
        if(GetComponentInChildren<MultipleTrigger>().onTrigger)
        {
            if (ObjectToAppear == null)
            {
                ObjectToAppear = new GameObject("ObjectToAppear");
            }
            ObjectToAppear.SetActive(true);

            if (ObjectToDisappear == null)
            {
                ObjectToDisappear = new GameObject("ObjectToDisappear");
            }
            ObjectToDisappear.SetActive(false);
        }
    }
}
