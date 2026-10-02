using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemEnter_ExitTrigger : MonoBehaviour
{
    [SerializeField] GameObject trigger1;
    [SerializeField] GameObject trigger2;
    [SerializeField] GameObject ObjectToAppear;
    [SerializeField] GameObject ObjectToDisappear;
    // Start is called before the first frame update
    private void OnEnable()
    {
        if (ObjectToAppear == null)
        {
            ObjectToAppear = new GameObject("ObjectToAppear");
        }
        if (ObjectToDisappear == null)
        {
            ObjectToDisappear = new GameObject("ObjectToDisappear");
        }

    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (trigger2.GetComponent<Enter_ExitTrigger>()._ExitTrigger)
        {
            ObjectToDisappear.gameObject.SetActive(false);
            ObjectToAppear.gameObject.SetActive(true);
            trigger2.gameObject.SetActive(false);
            trigger1.gameObject.SetActive(true);
        }

        if (trigger1.GetComponent<Enter_ExitTrigger>()._EnterTrigger)
        {
            ObjectToDisappear.gameObject.SetActive(true);
            ObjectToAppear.gameObject.SetActive(false);
            trigger1.gameObject.SetActive(false);
            trigger2.gameObject.SetActive(true);
        }
    }
}
